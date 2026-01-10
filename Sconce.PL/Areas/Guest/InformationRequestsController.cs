using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Guest
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Guest")]
    public class InformationRequestsController : ControllerBase
    {
        private readonly IInformationRequestService _informationRequestService;

        public InformationRequestsController(IInformationRequestService informationRequestService)
        {
            _informationRequestService = informationRequestService;
        }

        // Submit a new information request
        [HttpPost("Submit")]
        public async Task<ActionResult<Response>> SubmitRequest([FromBody] InformationRequestRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(new ErrorResponse { Errors = errors });
            }

            var result = await _informationRequestService.CreateAsync(request);
            if (result.NumberOfEntries == 0)
                return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
