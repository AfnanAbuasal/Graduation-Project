using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sconce.DAL.DTO.Responses;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IParentAccessService
    {
        Task<(bool HasAccess, string ErrorMessage)> ValidateParentAccessToStudentAsync(string parentId, string studentId);
        Task<Response> GetChildrenAsync(string parentId);
    }
}
