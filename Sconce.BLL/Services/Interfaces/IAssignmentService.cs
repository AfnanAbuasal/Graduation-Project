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
    public interface IAssignmentService : IFileGenericService<AssignmentRequest, AssignmentResponse, Assignment>
    {
        Task<Response> GetAllBySectionAsync(int sectionId, string instructorId);
    }
}
