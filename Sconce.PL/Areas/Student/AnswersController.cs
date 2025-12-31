using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Threading.Tasks;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswersController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        /// Creates or updates an answer to an exam question.
        /// For MCQ: requires SelectedChoiceIds
        /// For Essay: requires Text and/or File
        [HttpPost]
        public async Task<ActionResult<Response>> CreateAnswer([FromForm] AnswerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _answerService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) 
                return BadRequest(result.Response);
            
            return Ok(result.Response);
        }

        /// Retrieves all answers for a specific exam attempt.
        [HttpGet("Attempt/{attemptId}")]
        public async Task<ActionResult<Response>> GetMyAnswersForAttempt([FromRoute] int attemptId)
        {
            var result = await _answerService.GetMyAnswersForAttemptAsync(attemptId);
            
            if (result is ErrorResponse errorResponse)
                return BadRequest(errorResponse);
            
            return Ok(result);
        }
    }
}
