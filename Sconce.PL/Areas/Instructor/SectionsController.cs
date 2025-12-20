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
        private readonly IContentService _contentService;

        public SectionsController(ISectionService sectionService, IContentService contentService)
        {
            _sectionService = sectionService;
            _contentService = contentService;
        }

        // Lists sections assigned to the current instructor.
        [HttpGet("MySections")]
        public async Task<ActionResult<Response>> GetMySections([FromQuery] bool onlyActive = false, [FromQuery] string? sortBy = null)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(instructorId))
                return Unauthorized(new ErrorResponse { Errors = ["Unable to determine the current instructor."] });

            var result = await _sectionService.GetByInstructorAsync(instructorId, onlyActive, sortBy);
            return Ok(result);
        }

        // Gets all content items for a section, organized by week.
        [HttpGet("{sectionId}/Content")]
        public async Task<ActionResult<Response>> GetSectionContent([FromRoute] int sectionId)
        {
            var result = await _contentService.GetBySectionIdAsync(sectionId);
            return Ok(result);
        }
    }
}
