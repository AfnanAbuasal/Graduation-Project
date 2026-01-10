using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IInformationRequestService
    {
        Task<(int NumberOfEntries, Response Response)> CreateAsync(InformationRequestRequest request);
        Task<Response> GetAllAsync();
    }
}
