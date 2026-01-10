using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Super Admin")]
    public class InformationRequestsController : ControllerBase
    {
        private readonly IInformationRequestService _informationRequestService;

        public InformationRequestsController(IInformationRequestService informationRequestService)
        {
            _informationRequestService = informationRequestService;
        }

        // Get all information requests.
        [HttpGet]
        public async Task<ActionResult<Response>> GetAll()
        {
            var response = await _informationRequestService.GetAllAsync();
            return Ok(response);
        }
    }
}
