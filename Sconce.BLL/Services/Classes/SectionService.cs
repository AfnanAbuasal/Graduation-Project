using Mapster;
using Microsoft.AspNetCore.Identity;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public SectionService(ISectionRepository sectionRepository, ICourseRepository courseRepository, UserManager<ApplicationUser> userManager) : base(sectionRepository)
        {
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
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

        public async Task<Response> GetAllAsync(bool onlyActive = false)
        {
            var list = await _sectionRepository.GetAllWithInstructorAsync();

            if (onlyActive)
                list = list.Where(x => x.Status == Sconce.DAL.Models.Enums.Status.Active);

            var responseList = new List<SectionResponse>();

            foreach (var entity in list)
            {
                var dto = entity.Adapt<SectionResponse>();
                dto.CourseName = entity.Course?.Name;
                dto.InstructorName = entity.Instructor?.FullName;

                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<SectionResponse>> { Data = responseList };
        }

        public async Task<Response> GetByInstructorAsync(string instructorId, bool onlyActive = false)
        {
            var list = await _sectionRepository.GetByInstructorIdWithInstructorAsync(instructorId);

            if (onlyActive)
                list = list.Where(x => x.Status == Sconce.DAL.Models.Enums.Status.Active);

            var responseList = new List<SectionResponse>();

            foreach (var entity in list)
            {
                var dto = entity.Adapt<SectionResponse>();
                dto.CourseName = entity.Course?.Name;
                dto.InstructorName = entity.Instructor?.FullName;

                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<SectionResponse>> { Data = responseList };
        }

        public async Task<(bool Success, Response Response)> GetByIdAsync(int Id)
        {
            var entity = await _sectionRepository.GetByIdWithInstructorAsync(Id);
            if (entity == null)
                return (false, new ErrorResponse { Errors = ["Not Found."] });

            var dto = entity.Adapt<SectionResponse>();
            dto.CourseName = entity.Course?.Name;
            dto.InstructorName = entity.Instructor?.FullName;

            return (true, new SuccessResponse<SectionResponse> { Data = dto });
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
            await _sectionRepository.UpdateAsync(section);

            return (true, new SuccessResponse<string> { Data = $"Instructor assigned to section successfully." });
        }

        public async Task<(bool Success, Response Response)> UnassignInstructorAsync(int sectionId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return (false, new ErrorResponse { Errors = ["Section not found."] });

            section.InstructorId = null;
            await _sectionRepository.UpdateAsync(section);

            return (true, new SuccessResponse<string> { Data = "Instructor unassigned from section successfully." });
        }
    }
}
