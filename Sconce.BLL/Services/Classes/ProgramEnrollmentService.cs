using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Extensions;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class ProgramEnrollmentService : IProgramEnrollmentService
    {
        private readonly IProgramEnrollmentRepository _programEnrollmentRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IStudentSectionRepository _studentSectionRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly INotificationService _notificationService;

        public ProgramEnrollmentService(
            IProgramEnrollmentRepository programEnrollmentRepository,
            IProgramRepository programRepository,
            IStudentSectionRepository studentSectionRepository,
            ISectionRepository sectionRepository,
            INotificationService notificationService)
        {
            _programEnrollmentRepository = programEnrollmentRepository;
            _programRepository = programRepository;
            _studentSectionRepository = studentSectionRepository;
            _sectionRepository = sectionRepository;
            _notificationService = notificationService;
        }

        public async Task<(bool Success, Response Response)> EnrollStudentAsync(int programId, string studentId)
        {
            // Check if program exists
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return (false, new ErrorResponse { Errors = ["Program not found."] });

            // Check if student is already enrolled
            var existingEnrollment = await _programEnrollmentRepository
                .GetByProgramAndStudentAsync(programId, studentId);
            if (existingEnrollment != null)
                return (false, new ErrorResponse { Errors = ["Student is already enrolled in this program."] });

            // Create new enrollment
            var enrollment = new ProgramEnrollment
            {
                ProgramId = programId,
                StudentId = studentId,
                CreatedAt = DateTime.UtcNow
            };

            await _programEnrollmentRepository.AddAsync(enrollment);

            // Reload with details (includes navigation props) for mapping
            var createdEnrollment = await _programEnrollmentRepository.GetByIdAsync(enrollment.Id);

            if (createdEnrollment == null)
                return (false, new ErrorResponse { Errors = ["Failed to load created enrollment."] });

            // Set student age
            if (createdEnrollment.Student != null)
            {
                createdEnrollment.StudentAge = CalculateAge(createdEnrollment.Student.DateOfBirth);
                await _programEnrollmentRepository.UpdateAsync(createdEnrollment);

                // Notify student about enrollment and next steps
                if (program.HasProficiencyExam)
                {
                    await _notificationService.SendProgramEnrollmentWithExamAsync(createdEnrollment.Student, program);
                }
                else
                {
                    await _notificationService.SendProgramEnrollmentWithoutExamAsync(createdEnrollment.Student, program);
                }
            }

            var response = MapToResponse(createdEnrollment, program);

            return (true, new SuccessResponse<ProgramEnrollmentResponse> { Data = response });
        }

        public async Task<IEnumerable<ProgramEnrollmentResponse>> GetEnrollmentsForProgramAsync(
            int programId,
            string? placementStatus = null,
            string? examStatus = null,
            int? recommendedCourseId = null,
            string sortOrder = "oldest")
        {
            var enrollments = await _programEnrollmentRepository
                .GetFilteredEnrollmentsAsync(programId, placementStatus, examStatus, recommendedCourseId, sortOrder);

            var responses = enrollments
                .Select(enrollment => MapToResponse(enrollment, enrollment.Program))
                .ToList();

            return responses;
        }

        public async Task<(bool Success, Response Response)> SetRecommendedCourseAsync(int programId, string studentId, int recommendedCourseId)
        {
            var enrollment = await _programEnrollmentRepository.GetByProgramAndStudentAsync(programId, studentId, includeProficiencyExamAttempt: true);
            if (enrollment == null)
                return (false, new ErrorResponse { Errors = ["Enrollment not found for the specified program and student."] });

            if (enrollment.ProficiencyExamAttempt == null || enrollment.ProficiencyExamAttempt.AttemptStatus != AttemptStatus.Graded)
                return (false, new ErrorResponse { Errors = ["Proficiency exam attempt is not finalized (graded)."] });

            enrollment.RecommendedCourseId = recommendedCourseId;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _programEnrollmentRepository.UpdateAsync(enrollment);

            var updated = await _programEnrollmentRepository.GetByIdAsync(enrollment.Id);
            var response = MapToResponse(updated!, updated!.Program);
            return (true, new SuccessResponse<ProgramEnrollmentResponse> { Data = response });
        }

        public async Task<(bool Success, Response Response)> AddStudentToSectionAsync(int programEnrollmentId, int sectionId)
        {
            var enrollment = await _programEnrollmentRepository.GetByIdAsync(programEnrollmentId);
            if (enrollment == null)
                return (false, new ErrorResponse { Errors = ["Program enrollment not found."] });

            if (string.IsNullOrWhiteSpace(enrollment.StudentId))
                return (false, new ErrorResponse { Errors = ["Enrollment has no associated student."] });

            // Validate proficiency exam requirements
            if (enrollment.ProficiencyExamAttempt == null)
                return (false, new ErrorResponse { Errors = ["Proficiency exam attempt not found."] });

            if (enrollment.ProficiencyExamAttempt.AttemptStatus != AttemptStatus.Graded)
                return (false, new ErrorResponse { Errors = ["Proficiency exam must be graded before placing student in a section."] });

            if (!enrollment.ProficiencyExamAttempt.Score.HasValue)
                return (false, new ErrorResponse { Errors = ["Exam score is required."] });

            if (!enrollment.ProficiencyExamAttempt.MaxScore.HasValue)
                return (false, new ErrorResponse { Errors = ["Exam max score is required."] });

            // Validate recommended course requirements
            if (!enrollment.RecommendedCourseId.HasValue)
                return (false, new ErrorResponse { Errors = ["Recommended course ID is required."] });

            if (enrollment.RecommendedCourse == null || string.IsNullOrWhiteSpace(enrollment.RecommendedCourse.Name))
                return (false, new ErrorResponse { Errors = ["Recommended course must be assigned before placing student in a section."] });

            var section = await _sectionRepository.GetByIdWithCourseAsync(sectionId);
            if (section == null)
                return (false, new ErrorResponse { Errors = ["Section not found."] });

            var sectionProgramId = section.Course?.Level?.ProgramId;
            if (!sectionProgramId.HasValue || sectionProgramId.Value != enrollment.ProgramId)
                return (false, new ErrorResponse { Errors = ["Section does not belong to the same program as the enrollment."] });

            var alreadyInSection = await _studentSectionRepository.ExistsAsync(enrollment.StudentId, sectionId);
            if (alreadyInSection)
                return (false, new ErrorResponse { Errors = ["Student is already placed in this section."] });

            if (section.CurrentCapacity >= section.Capacity)
                return (false, new ErrorResponse { Errors = ["Section capacity has been reached."] });

            var studentSection = new StudentSection
            {
                StudentId = enrollment.StudentId,
                SectionId = sectionId,
                AddedAt = DateTime.UtcNow
            };

            await _studentSectionRepository.AddAsync(studentSection);

            section.CurrentCapacity++;
            await _sectionRepository.UpdateAsync(section);

            enrollment.PlacedSectionId = sectionId;
            enrollment.UpdatedAt = DateTime.UtcNow;
            await _programEnrollmentRepository.UpdateAsync(enrollment);

            var updatedEnrollment = await _programEnrollmentRepository.GetByIdAsync(programEnrollmentId);
            if (updatedEnrollment?.Student != null)
            {
                await _notificationService.SendStudentPlacedInSectionAsync(updatedEnrollment.Student, section);
            }

            var response = MapToResponse(updatedEnrollment!, updatedEnrollment!.Program);
            return (true, new SuccessResponse<ProgramEnrollmentResponse> { Data = response });
        }

        private ProgramEnrollmentResponse MapToResponse(ProgramEnrollment enrollment, Program? program)
        {
            return new ProgramEnrollmentResponse
            {
                Id = enrollment.Id,
                ProgramId = enrollment.ProgramId,
                ProgramName = program?.Name ?? enrollment.Program?.Name,
                StudentId = enrollment.StudentId,
                StudentFullName = enrollment.Student?.FullName,
                StudentAge = enrollment.StudentAge ?? (enrollment.Student != null ? CalculateAge(enrollment.Student.DateOfBirth) : null),
                CreatedAt = enrollment.CreatedAt,
                ProficiencyExamStatusDisplay = enrollment.ProficiencyExamAttempt != null
                    ? enrollment.ProficiencyExamAttempt.AttemptStatus.ToDisplayString()
                    : "Not Taken",
                ExamScore = enrollment.ProficiencyExamAttempt?.Score,
                ExamMaxScore = enrollment.ProficiencyExamAttempt?.MaxScore,
                ExamScorePercentage = CalculateExamScorePercentage(enrollment.ProficiencyExamAttempt?.Score,
                                                                   enrollment.ProficiencyExamAttempt?.MaxScore),
                RecommendedCourseId = enrollment.RecommendedCourseId,
                RecommendedCourseName = enrollment.RecommendedCourse?.Name,
                PlacedSectionId = enrollment.PlacedSectionId,
                PlacedSectionName = enrollment.PlacedSection?.Name,
                EvaluatedByInstructorName = enrollment.EvaluatedByInstructor?.FullName,
                EvaluatedAt = enrollment.EvaluatedAt
            };
        }
        private decimal? CalculateExamScorePercentage(decimal? score, decimal? maxScore)
        {
            if (score.HasValue && maxScore.HasValue && maxScore.Value > 0)
            {
                return Math.Round(score.Value / maxScore.Value * 100, 2);
            }
            return null;
        }

        private int? CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }
}
