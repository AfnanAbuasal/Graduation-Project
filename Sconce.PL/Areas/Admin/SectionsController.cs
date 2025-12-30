using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class SectionsController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionsController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        // Lists all sections, optionally only the active ones.
        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
        {
            var sections = await _sectionService.GetAllAsync(onlyActive);
            return Ok(sections);
        }

        // Lists all sections in a specific course, optionally only the active ones.
        [HttpGet("Course/{courseId}")]
        public async Task<ActionResult<Response>> GetByCourse([FromRoute] int courseId, [FromQuery] bool onlyActive = false)
        {
            var sections = await _sectionService.GetByCourseAsync(courseId, onlyActive);
            return Ok(sections);
        }

        // Shows details for a specific section.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _sectionService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Lists sections assigned to a specific instructor.
        [HttpGet("ByInstructor/{instructorId}")]
        public async Task<ActionResult<Response>> GetByInstructor([FromRoute] string instructorId, [FromQuery] bool onlyActive = false)
        {
            if (string.IsNullOrWhiteSpace(instructorId))
                return BadRequest(new ErrorResponse { Errors = ["InstructorId is required."] });

            var result = await _sectionService.GetByInstructorAsync(instructorId, onlyActive);
            return Ok(result);
        }

        // Adds a new section.
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] SectionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _sectionService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing section.
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] SectionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _sectionService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Removes a section.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _sectionService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Enables or disables a section.
        [HttpPatch("{id}/ToggleStatus")]
        public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
        {
            var result = await _sectionService.ToggleStatusAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Assigns an instructor to a section.
        [HttpPatch("{id}/AssignInstructor")]
        public async Task<ActionResult<Response>> AssignInstructor([FromRoute] int id, [FromBody] AssignInstructorRequest request)
        {
            var result = await _sectionService.AssignInstructorAsync(id, request.InstructorId);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Removes an instructor assignment from a section.
        [HttpPatch("{id}/UnassignInstructor")]
        public async Task<ActionResult<Response>> UnassignInstructor([FromRoute] int id)
        {
            var result = await _sectionService.UnassignInstructorAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Increases the capacity of a section.
        [HttpPatch("{id}/IncreaseCapacity")]
        public async Task<ActionResult<Response>> IncreaseCapacity([FromRoute] int id, [FromBody] IncreaseCapacityRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _sectionService.IncreaseCapacityAsync(id, request.AdditionalCapacity);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
