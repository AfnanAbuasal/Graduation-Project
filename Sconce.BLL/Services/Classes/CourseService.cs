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

        public override async Task<int> CreateAsync(CourseRequest request)
        {
            // Ensure Program exists before creating course
            var program = await _programRepository.GetByIdAsync(request.ProgramId);
            if (program == null)
                throw new InvalidOperationException("Program not found. Please create the program first.");

            var course = request.Adapt<Course>();
            return await _courseRepository.AddAsync(course);
        }
    }
}
