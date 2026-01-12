using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        // Shows details for a specific assignment.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _assignmentService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Get my assignment performance.
        [HttpGet("Performance/Section/{sectionId}")]
        public async Task<ActionResult<Response>> GetMyPerformance([FromRoute] int sectionId, [FromQuery] int? windowDays = null)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            var request = new PerformanceFilterRequest
            {
                SectionId = sectionId,
                StudentId = studentId,
                WindowDays = windowDays
            };

            var result = await _assignmentService.GetStudentAssignmentPerformanceAsync(request);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
