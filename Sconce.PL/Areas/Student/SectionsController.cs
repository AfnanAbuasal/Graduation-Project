using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class SectionsController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly ISectionService _sectionService;

        public SectionsController(IContentService contentService, ISectionService sectionService)
        {
            _contentService = contentService;
            _sectionService = sectionService;
        }

        // Gets all content items for a section, organized by week.
        [HttpGet("{sectionId}/Content")]
        public async Task<ActionResult<Response>> GetSectionContent([FromRoute] int sectionId)
        {
            var result = await _contentService.GetBySectionIdAsync(sectionId);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }

        // Gets all sections for the authenticated student.
        [HttpGet("MySections")]
        public async Task<ActionResult<Response>> GetMySections()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            var result = await _sectionService.GetByStudentAsync(studentId);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
