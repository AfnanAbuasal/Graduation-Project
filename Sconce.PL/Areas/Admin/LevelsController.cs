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
    public class LevelsController : ControllerBase
    {
        private readonly ILevelService _levelService;
        public LevelsController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        // Lists all levels, optionally only the active ones.
        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
        {
            var levels = await _levelService.GetAllAsync(onlyActive);
            return Ok(levels);
        }

        // Shows details for a specific level.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _levelService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Adds a new level.
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] LevelRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _levelService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing level.
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] LevelRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _levelService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Removes a level.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _levelService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Enables or disables a level.
        [HttpPatch("{id}/ToggleStatus")]
        public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
        {
            var result = await _levelService.ToggleStatusAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Increases the planned course count for a level.
        [HttpPatch("{id}/IncreasePlannedCourseCount")]
        public async Task<ActionResult<Response>> IncreasePlannedCourseCount([FromRoute] int id, [FromBody] IncreasePlannedCountRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _levelService.IncreasePlannedCourseCountAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
