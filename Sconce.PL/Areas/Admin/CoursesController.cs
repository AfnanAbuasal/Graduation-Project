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
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // Lists all courses, optionally only the active ones.
        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
        {
            var courses = await _courseService.GetAllAsync(onlyActive);
            return Ok(courses);
        }

        // Shows details for a specific course.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _courseService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Adds a new course.
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _courseService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing course.
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _courseService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Removes a course.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _courseService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Enables or disables a course.
        [HttpPatch("{id}/ToggleStatus")]
        public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
        {
            var result = await _courseService.ToggleStatusAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Gets the course in a specific level, optionally only active ones.
        [HttpGet("Level/{levelId}")]
        public async Task<ActionResult<Response>> GetByLevel([FromRoute] int levelId, bool onlyActive = false)
        {
            var result = await _courseService.GetByLevelAsync(levelId, onlyActive);
            return Ok(result);
        }

        // Gets all ordered courses in a program.
        [HttpGet("Program/{programId}")]
        public async Task<ActionResult<Response>> GetByProgram([FromRoute] int programId, [FromQuery] bool onlyActive = false)
        {
            var result = await _courseService.GetByProgramAsync(programId, onlyActive);
            return Ok(result);
        }

        // Gets the count of courses in a program.
        [HttpGet("Program/{programId}/Count")]
        public async Task<ActionResult<Response>> GetCourseCountByProgram([FromRoute] int programId, [FromQuery] bool onlyActive = false)
        {
            var result = await _courseService.GetCourseCountByProgramAsync(programId, onlyActive);
            return Ok(result);
        }
    }
}
