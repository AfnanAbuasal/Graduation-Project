using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface ISectionService : IGenericService<SectionRequest, SectionResponse, Section>
    {
        Task<(bool Success, Response Response)> AssignInstructorAsync(int sectionId, string instructorId);
        Task<(bool Success, Response Response)> UnassignInstructorAsync(int sectionId);
        Task<Response> GetByInstructorAsync(string instructorId, bool onlyActive = false, string? sortBy = null);
        Task<Response> GetByCourseAsync(int courseId, bool onlyActive = false);
        Task<(bool Success, Response Response)> IncreaseCapacityAsync(int sectionId, int additionalCapacity);
        Task<Response> GetByStudentAsync(string studentId);
    }
}
