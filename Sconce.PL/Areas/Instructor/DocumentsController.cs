using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // Gets all documents for a section.
        [HttpGet("Section/{sectionId}")]
        public async Task<ActionResult<Response>> GetBySection([FromRoute] int sectionId)
        {
            var result = await _documentService.GetAllBySectionIdAsync(sectionId);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }

        // Shows details for a specific document.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _documentService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Creates a new document.
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromForm] DocumentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _documentService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing document.
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromForm] DocumentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _documentService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Deletes a document.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _documentService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Enables or disables a document.
        [HttpPatch("{id}/ToggleStatus")]
        public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
        {
            var result = await _documentService.ToggleStatusAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
