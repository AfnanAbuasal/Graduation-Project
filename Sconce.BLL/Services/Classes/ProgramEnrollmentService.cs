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
        private readonly INotificationService _notificationService;

        public ProgramEnrollmentService(
            IProgramEnrollmentRepository programEnrollmentRepository,
            IProgramRepository programRepository,
            INotificationService notificationService)
        {
            _programEnrollmentRepository = programEnrollmentRepository;
            _programRepository = programRepository;
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

            // Notify student about enrollment and next steps
            if (createdEnrollment?.Student != null)
            {
                if (program.HasProficiencyExam)
                {
                    await _notificationService.SendProgramEnrollmentWithExamAsync(createdEnrollment.Student, program);
                }
                else
                {
                    await _notificationService.SendProgramEnrollmentWithoutExamAsync(createdEnrollment.Student, program);
                }
            }
            var response = MapToResponse(createdEnrollment!, program);

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

        private ProgramEnrollmentResponse MapToResponse(ProgramEnrollment enrollment, Program? program)
        {
            return new ProgramEnrollmentResponse
            {
                Id = enrollment.Id,
                ProgramId = enrollment.ProgramId,
                ProgramName = program?.Name ?? enrollment.Program?.Name,
                StudentId = enrollment.StudentId,
                StudentFullName = enrollment.Student?.FullName,
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
    }
}
