using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class SectionsController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionsController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        // GET: api/Instructor/Sections/My?onlyActive=true
        [HttpGet("My")]
        public async Task<ActionResult<Response>> GetMySections([FromQuery] bool onlyActive = false)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(instructorId))
                return Unauthorized(new ErrorResponse { Errors = ["Unable to determine the current instructor."] });

            var result = await _sectionService.GetByInstructorAsync(instructorId, onlyActive);
            return Ok(result);
        }
    }
}
