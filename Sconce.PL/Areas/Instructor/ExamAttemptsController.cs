using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using System.Threading.Tasks;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class ExamAttemptsController : ControllerBase
    {
        private readonly IExamAttemptService _examAttemptService;

        public ExamAttemptsController(IExamAttemptService examAttemptService)
        {
            _examAttemptService = examAttemptService;
        }

        /// Get all student attempts for a specific exam (instructor view).
        [HttpGet("Exam/{examId}")]
        public async Task<ActionResult<Response>> GetByExamId([FromRoute] int examId)
        {
            var result = await _examAttemptService.GetAttemptsByExamIdAsync(examId);
            if (result is ErrorResponse)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
