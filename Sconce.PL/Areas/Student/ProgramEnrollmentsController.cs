using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class ProgramEnrollmentsController : ControllerBase
    {
        private readonly IProgramEnrollmentService _programEnrollmentService;

        public ProgramEnrollmentsController(IProgramEnrollmentService programEnrollmentService)
        {
            _programEnrollmentService = programEnrollmentService;
        }

        // Enroll in a program
        [HttpPost("{programId}/Enroll")]
        public async Task<ActionResult<Response>> EnrollInProgram([FromRoute] int programId)
        {
            // Extract student ID from JWT token
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrWhiteSpace(studentId))
                return Unauthorized("Not authenticated.");

            var (success, response) = await _programEnrollmentService.EnrollStudentAsync(programId, studentId);
            if (!success) return BadRequest(response);
            
            return Ok(response);
        }
    }
}
