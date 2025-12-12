using Mapster;
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
        private readonly IGenericRepository<Instructor> _instructorRepository;

        public SectionService(ISectionRepository sectionRepository, ICourseRepository courseRepository, IGenericRepository<Instructor> instructorRepository) : base(sectionRepository)
        {
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(SectionRequest request)
        {
            // Ensure the course exists before adding section
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null) return (0, new ErrorResponse { Errors = [$"Course with Id: {request.CourseId} not found."] });

            // Validate instructor if provided
            if (!string.IsNullOrEmpty(request.InstructorId))
            {
                var instructor = await _instructorRepository.GetByIdAsync(request.InstructorId);
                if (instructor == null)
                    return (0, new ErrorResponse { Errors = [$"Instructor with Id: {request.InstructorId} not found."] });
            }

            var section = request.Adapt<Section>();
            var rows = await _sectionRepository.AddAsync(section);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
        }

        public async Task<(bool Success, Response Response)> AssignInstructorAsync(int sectionId, string instructorId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return (false, new ErrorResponse { Errors = ["Section not found."] });

            var instructor = await _instructorRepository.GetByIdAsync(instructorId);
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
