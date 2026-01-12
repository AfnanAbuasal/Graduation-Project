using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
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

        // Get all student attempts for a specific exam (instructor view).
        [HttpGet("Exam/{examId}")]
        public async Task<ActionResult<Response>> GetByExamId([FromRoute] int examId)
        {
            var result = await _examAttemptService.GetAttemptsByExamIdAsync(examId);
            if (result is ErrorResponse)
                return BadRequest(result);
            return Ok(result);
        }

        // Get full details of a single exam attempt including questions and answers.
        // Returns detailed attempt information with all questions and student answers.
        [HttpGet("{attemptId}")]
        public async Task<ActionResult<Response>> GetAttemptDetails([FromRoute] int attemptId)
        {
            var result = await _examAttemptService.GetAttemptDetailsAsync(attemptId);
            if (!result.Success)
                return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Finalizes an exam attempt after grading all essays.
        // Computes final score and marks attempt as graded.
        [HttpPost("{attemptId}/Finalize")]
        public async Task<ActionResult<Response>> FinalizeAttempt([FromRoute] int attemptId)
        {
            var result = await _examAttemptService.FinalizeAttemptAsync(attemptId);
            if (!result.Success)
                return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Get student's exam performance.
        [HttpPost("Performance")]
        public async Task<ActionResult<Response>> GetStudentPerformance([FromBody] PerformanceFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
        
            var result = await _examAttemptService.GetStudentExamPerformanceAsync(request);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
