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
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // Shows details for a specific document.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _documentService.GetByIdAsync(id);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
