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
    public class ProgramController : ControllerBase
    {
        private readonly IProgramService _programService;
        public ProgramController(IProgramService programService)
        {
            _programService = programService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
        {
            var programs = await _programService.GetAllAsync(onlyActive);
            return Ok(programs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var program = await _programService.GetByIdAsync(id);
            if (program == null)
                return NotFound("Program not found.");

            return Ok(program);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _programService.CreateAsync(request);
            if (id <= 0)
                return BadRequest("Failed to create program.");

            // Optionally fetch the created program for the response
            var created = await _programService.GetByIdAsync(id);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _programService.UpdateAsync(id, request);
            if (result <= 0)
                return NotFound("Program not found or update failed.");

            return Ok("Program updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _programService.DeleteAsync(id);
            if (result <= 0)
                return NotFound("Program not found or delete failed.");

            return Ok("Program deleted successfully.");
        }

        [HttpPatch("{id}/ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromRoute] int id)
        {
            var success = await _programService.ToggleStatusAsync(id);
            if (!success)
                return NotFound("Program not found.");

            return Ok("Program status toggled successfully.");
        }
    }
}
