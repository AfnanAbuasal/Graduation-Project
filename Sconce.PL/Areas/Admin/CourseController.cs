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
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
        {
            var courses = await _courseService.GetAllAsync(onlyActive);
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound("Course not found.");
            return Ok(course);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _courseService.CreateAsync(request);
            if (id <= 0)
                return BadRequest("Failed to create course.");

            var created = await _courseService.GetByIdAsync(id);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _courseService.UpdateAsync(id, request);
            if (result <= 0)
                return NotFound("Course not found or update failed.");

            return Ok("Course updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _courseService.DeleteAsync(id);
            if (result <= 0)
                return NotFound("Course not found or delete failed.");

            return Ok("Course deleted successfully.");
        }

        [HttpPatch("{id}/ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromRoute] int id)
        {
            var success = await _courseService.ToggleStatusAsync(id);
            if (!success)
                return NotFound("Course not found.");

            return Ok("Course status toggled successfully.");
        }
    }
}
