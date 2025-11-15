using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;

namespace Sconce.PL.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Super Admin")]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
        {
            var sections = await _sectionService.GetAllAsync(onlyActive);
            return Ok(sections);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var section = await _sectionService.GetByIdAsync(id);
            if (section == null)
                return NotFound("Section not found.");

            return Ok(section);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SectionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _sectionService.CreateAsync(request);
            if (id <= 0)
                return BadRequest("Failed to create section.");

            var created = await _sectionService.GetByIdAsync(id);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SectionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _sectionService.UpdateAsync(id, request);
            if (result <= 0)
                return NotFound("Section not found or update failed.");

            return Ok("Section updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _sectionService.DeleteAsync(id);
            if (result <= 0)
                return NotFound("Section not found or delete failed.");

            return Ok("Section deleted successfully.");
        }

        [HttpPatch("{id}/ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromRoute] int id)
        {
            var success = await _sectionService.ToggleStatusAsync(id);
            if (!success)
                return NotFound("Section not found.");

            return Ok("Section status toggled successfully.");
        }
    }
}
