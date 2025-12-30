using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using System.Threading.Tasks;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class ExamAttemptsController : ControllerBase
    {
        private readonly IExamAttemptService _examAttemptService;

        public ExamAttemptsController(IExamAttemptService examAttemptService)
        {
            _examAttemptService = examAttemptService;
        }

        // Starts a new exam attempt for the authenticated student (or fetches an In-Progress one).
        [HttpPost("Start/{examId}")]
        public async Task<ActionResult<Response>> StartAttempt([FromRoute] int examId)
        {
            var result = await _examAttemptService.StartAttemptAsync(examId);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Gets all attempts for a specific exam by the authenticated student.
        [HttpGet("Exam/{examId}")]
        public async Task<ActionResult<Response>> GetMyAttempts([FromRoute] int examId)
        {
            var result = await _examAttemptService.GetMyAttemptsAsync(examId);
            return Ok(result);
        }

        // Submits an exam attempt.
        [HttpPost("{attemptId}/Submit")]
        public async Task<ActionResult<Response>> SubmitAttempt([FromRoute] int attemptId)
        {
            var result = await _examAttemptService.SubmitAttemptAsync(attemptId);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
