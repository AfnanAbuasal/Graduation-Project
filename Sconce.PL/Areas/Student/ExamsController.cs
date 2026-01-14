using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        // Shows details for a specific exam.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _examService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Gets all published exams for a specific section with student-specific state.
        [HttpGet("Section/{sectionId}")]
        public async Task<ActionResult<Response>> GetPublishedBySection([FromRoute] int sectionId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(studentId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            var result = await _examService.GetPublishedBySectionForStudentAsync(sectionId, studentId);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
