using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class SectionsController : ControllerBase
    {
        private readonly IContentService _contentService;

        public SectionsController(IContentService contentService)
        {
            _contentService = contentService;
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
