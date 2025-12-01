using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

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
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
        {
            var programs = await _programService.GetAllAsync(onlyActive);
            return Ok(programs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _programService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] ProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _programService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
            //return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] ProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _programService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _programService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpPatch("{id}/ToggleStatus")]
        public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
        {
            var result = await _programService.ToggleStatusAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
