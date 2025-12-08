using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IDropoutService : IGenericService<DropoutRequest, DropoutResponse, Dropout>
    {
        Task<(bool Success, Response Response)> ReviewDropoutAsync(int requestId, ApplicationStatus newStatus, string feedback);
    }
}
