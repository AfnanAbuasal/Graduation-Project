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
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
        {
            var courses = await _courseService.GetAllAsync(onlyActive);
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _courseService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _courseService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _courseService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _courseService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpPatch("{id}/ToggleStatus")]
        public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
        {
            var result = await _courseService.ToggleStatusAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
