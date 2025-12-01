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
        public SectionService(ISectionRepository sectionRepository, ICourseRepository courseRepository) : base(sectionRepository)
        {
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(SectionRequest request)
        {
            // Ensure the course exists before adding section
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null) return (0, new Response { Message = "Course not found. Please create the course first." });

            var section = request.Adapt<Section>();
            var number = await _sectionRepository.AddAsync(section);
            return (number, new Response { Message = $"{number} record(s) created successfully." });
        }
    }
}
