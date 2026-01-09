using Mapster;
using Microsoft.AspNetCore.Identity;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Repositories.Classes;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class SectionService : GenericService<SectionRequest, SectionResponse, Section>, ISectionService
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentSectionRepository _studentSectionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUrlHelper _urlHelper;

        public SectionService(ISectionRepository sectionRepository, ICourseRepository courseRepository, IStudentSectionRepository studentSectionRepository, UserManager<ApplicationUser> userManager, IUrlHelper urlHelper) : base(sectionRepository)
        {
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
            _studentSectionRepository = studentSectionRepository;
            _userManager = userManager;
            _urlHelper = urlHelper;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(SectionRequest request)
        {
            // Ensure the course exists before adding section
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null) return (0, new ErrorResponse { Errors = [$"Course with Id: {request.CourseId} not found."] });

            // Validate instructor if provided
            if (!string.IsNullOrEmpty(request.InstructorId))
            {
                var instructor = await _userManager.FindByIdAsync(request.InstructorId);
                if (instructor == null)
                    return (0, new ErrorResponse { Errors = [$"Instructor with Id: {request.InstructorId} not found."] });
            }

            var section = request.Adapt<Section>();
            var rows = await _sectionRepository.AddAsync(section);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
        }

        public override async Task<Response> GetAllAsync(bool onlyActive = false)
        {
            var list = await _sectionRepository.GetAllWithInstructorAsync();

            if (onlyActive)
                list = list.Where(x => x.Status == Status.Active);

            var responseList = new List<SectionResponse>();

            foreach (var entity in list)
            {
                var dto = entity.Adapt<SectionResponse>();
                dto.CourseName = entity.Course?.Name;
                dto.StartDate = entity.Course?.StartDate ?? default;
                dto.EndDate = entity.Course?.EndDate ?? default;
                dto.InstructorName = entity.Instructor?.FullName;

                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<SectionResponse>> { Data = responseList };
        }

        public async Task<Response> GetByInstructorAsync(string instructorId, bool onlyActive = false, string? sortBy = null)
        {
            var list = await _sectionRepository.GetByInstructorIdWithInstructorAsync(instructorId);

            if (onlyActive)
                list = list.Where(x => x.Status == Status.Active);

            // Apply sorting
            list = sortBy?.ToLower() switch
            {
                "name" => list.OrderBy(x => x.Course?.Name),
                "lastaccessed" => list.OrderByDescending(x => x.UpdatedAt),
                _ => list
            };

            var responseList = new List<SectionResponse>();

            foreach (var entity in list)
            {
                var dto = entity.Adapt<SectionResponse>();
                dto.CourseName = entity.Course?.Name;
                dto.StartDate = entity.Course?.StartDate ?? default;
                dto.EndDate = entity.Course?.EndDate ?? default;
                dto.InstructorName = entity.Instructor?.FullName;

                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<SectionResponse>> { Data = responseList };
        }

        public override async Task<(bool Success, Response Response)> GetByIdAsync(int Id)
        {
            var entity = await _sectionRepository.GetByIdWithInstructorAsync(Id);
            if (entity == null)
                return (false, new ErrorResponse { Errors = ["Not Found."] });

            var dto = entity.Adapt<SectionResponse>();
            dto.CourseName = entity.Course?.Name;
            dto.StartDate = entity.Course?.StartDate ?? default;
            dto.EndDate = entity.Course?.EndDate ?? default;
            dto.InstructorName = entity.Instructor?.FullName;

            return (true, new SuccessResponse<SectionResponse> { Data = dto });
        }

        public async Task<Response> GetByCourseAsync(int courseId, bool onlyActive = false)
        {
            var list = await _sectionRepository.GetByCourseIdAsync(courseId, onlyActive);

            var responseList = new List<SectionResponse>();

            foreach (var entity in list)
            {
                var dto = entity.Adapt<SectionResponse>();
                dto.CourseName = entity.Course?.Name;
                dto.StartDate = entity.Course?.StartDate ?? default;
                dto.EndDate = entity.Course?.EndDate ?? default;
                dto.InstructorName = entity.Instructor?.FullName;

                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<SectionResponse>> { Data = responseList };
        }

        public async Task<(bool Success, Response Response)> AssignInstructorAsync(int sectionId, string instructorId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return (false, new ErrorResponse { Errors = ["Section not found."] });

            var instructor = await _userManager.FindByIdAsync(instructorId);
            if (instructor == null)
                return (false, new ErrorResponse { Errors = ["Instructor not found."] });

            section.InstructorId = instructorId;
            section.UpdatedAt = DateTime.UtcNow;
            await _sectionRepository.UpdateAsync(section);

            return (true, new SuccessResponse<string> { Data = $"Instructor assigned to section successfully." });
        }

        public async Task<(bool Success, Response Response)> UnassignInstructorAsync(int sectionId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return (false, new ErrorResponse { Errors = ["Section not found."] });

            section.InstructorId = null;
            section.UpdatedAt = DateTime.UtcNow;
            await _sectionRepository.UpdateAsync(section);

            return (true, new SuccessResponse<string> { Data = "Instructor unassigned from section successfully." });
        }

        public async Task<(bool Success, Response Response)> IncreaseCapacityAsync(int sectionId, int additionalCapacity)
        {
            if (additionalCapacity <= 0)
                return (false, new ErrorResponse { Errors = ["Additional capacity must be greater than 0."] });

            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return (false, new ErrorResponse { Errors = ["Section not found."] });

            section.Capacity += additionalCapacity;
            section.UpdatedAt = DateTime.UtcNow;
            await _sectionRepository.UpdateAsync(section);

            return (true, new SuccessResponse<string> { Data = $"Section capacity increased by {additionalCapacity}. New capacity: {section.Capacity}." });
        }

        public async Task<Response> GetByStudentAsync(string studentId)
        {
            var studentSections = await _studentSectionRepository.GetByStudentIdAsync(studentId);

            var responseList = new List<StudentSectionResponse>();

            foreach (var studentSection in studentSections)
            {
                var section = studentSection.Section;
                var course = section?.Course;
                var level = course?.Level;
                var program = level?.Program;

                var dto = new StudentSectionResponse
                {
                    SectionId = section?.Id ?? 0,
                    SectionName = section?.Name ?? string.Empty,
                    CourseId = course?.Id ?? 0,
                    CourseName = course?.Name ?? string.Empty,
                    LevelId = level?.Id ?? 0,
                    LevelName = level?.Name ?? string.Empty,
                    ProgramId = program?.Id ?? 0,
                    ProgramName = program?.Name ?? string.Empty
                };

                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<StudentSectionResponse>> { Data = responseList };
        }

        public async Task<Response> GetStudentsBySectionIdAsync(int sectionId)
        {
            // Validate section exists
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return new ErrorResponse { Errors = ["Section not found."] };

            // Get students in the section
            var students = await _studentSectionRepository.GetStudentsBySectionIdAsync(sectionId);

            var responseList = new List<StudentProfileResponse>();

            foreach (var student in students)
            {
                var dto = student.Adapt<StudentProfileResponse>();
                dto.DocumentUrl = _urlHelper.BuildUrl(student.DocumentPath);
                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<StudentProfileResponse>> { Data = responseList };
        }
    }
}
