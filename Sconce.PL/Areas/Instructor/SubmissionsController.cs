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
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionsController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        // Gets all submissions for a specific assignment (scoped to instructor's section).
        [HttpGet("Assignment/{assignmentId}")]
        public async Task<ActionResult<Response>> GetByAssignment([FromRoute] int assignmentId)
        {
            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            var result = await _submissionService.GetAllByAssignmentAsync(assignmentId, instructorId);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }

        // Shows details for a specific submission.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _submissionService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Grades a student submission.
        [HttpPatch("{id}/Grade")]
        public async Task<ActionResult<Response>> Grade([FromRoute] int id, [FromBody] GradeSubmissionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _submissionService.GradeSubmissionAsync(id, request);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
