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
    public class InformationRequestService : IInformationRequestService
    {
        private readonly IInformationRequestRepository _repository;

        public InformationRequestService(IInformationRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<(int NumberOfEntries, Response Response)> CreateAsync(InformationRequestRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                    return (0, new ErrorResponse { Errors = ["Request cannot be null."] });

                // Map to entity
                var entity = request.Adapt<InformationRequest>();

                // Save to database
                var rows = await _repository.AddAsync(entity);

                if (rows > 0)
                {
                    return (rows, new SuccessResponse<string>
                    {
                        Data = "Thank you for contacting us, we will reach out shortly."
                    });
                }

                return (0, new ErrorResponse { Errors = ["Failed to submit request."] });
            }
            catch (Exception ex)
            {
                return (0, new ErrorResponse { Errors = [$"An error occurred: {ex.Message}"] });
            }
        }

        public async Task<Response> GetAllAsync()
        {
            try
            {
                var requests = await _repository.GetAllOrderedAsync();
                var responseDtos = requests.Adapt<List<InformationRequestResponse>>();

                return new SuccessResponse<List<InformationRequestResponse>>
                {
                    Data = responseDtos
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponse { Errors = [$"An error occurred: {ex.Message}"] };
            }
        }
    }
}
