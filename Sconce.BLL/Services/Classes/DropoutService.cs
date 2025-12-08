using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class DropoutService : GenericService<DropoutRequest, DropoutResponse, Dropout>, IDropoutService
    {
        private readonly IDropoutRepository _dropoutRepository;

        public DropoutService(IDropoutRepository dropoutRepository) : base(dropoutRepository)
        {
            _dropoutRepository = dropoutRepository;
        }

        public async Task<(bool Success, Response Response)> ReviewDropoutAsync(int requestId, ApplicationStatus newStatus, string feedback)
        {
            var request = await _dropoutRepository.GetByIdAsync(requestId);

            if (request == null)
                return (false, new ErrorResponse { Errors = ["Dropout request not found."] });

            if (request.ApplicationStatus != ApplicationStatus.Pending)
                return (false, new ErrorResponse { Errors = ["Only pending dropout requests can be reviewed."] });

            request.ApplicationStatus = newStatus;
            request.UpdatedAt = DateTime.UtcNow;

            await _dropoutRepository.UpdateAsync(request);

            var message = newStatus == ApplicationStatus.Approved
                ? "Dropout request approved."
                : "Dropout request rejected.";

            return (true, new SuccessResponse<string> { Data = message });
        }
    }
}
