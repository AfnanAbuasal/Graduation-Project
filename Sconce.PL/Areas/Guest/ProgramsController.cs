using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Guest
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Guest")]
    public class ProgramsController : ControllerBase
    {
        private readonly IProgramService _programService;
        public ProgramsController(IProgramService programService)
        {
            _programService = programService;
        }

        // Lists all active programs
        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = true)
        {
            var programs = await _programService.GetAllAsync(onlyActive);
            return Ok(programs);
        }
    }
}
