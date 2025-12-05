using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class CourseService : GenericService<CourseRequest, CourseResponse, Course>, ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IProgramRepository _programRepository;
        public CourseService(ICourseRepository courseRepository, IProgramRepository programRepository) : base(courseRepository)
        {
            _courseRepository = courseRepository;
            _programRepository = programRepository;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(CourseRequest request)
        {
            // Ensure Program exists before creating course
            var program = await _programRepository.GetByIdAsync(request.ProgramId);
            if (program == null) return (0, new ErrorResponse { Errors = [$"Program with Id: {request.ProgramId} not found."] });

            var course = request.Adapt<Course>();
            var rows = await _courseRepository.AddAsync(course);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
        }
    }
}
