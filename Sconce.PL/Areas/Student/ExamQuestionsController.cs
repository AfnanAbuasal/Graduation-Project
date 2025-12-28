using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using System.Threading.Tasks;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class ExamQuestionsController : ControllerBase
    {
        private readonly IExamQuestionService _examQuestionService;

        public ExamQuestionsController(IExamQuestionService examQuestionService)
        {
            _examQuestionService = examQuestionService;
        }

        // Gets all exam questions with full details for a published exam (student view).
        // Includes questions and choices, but does NOT include correct answers.
        // Validates exam is published and within availability window.
        [HttpGet("Exam/{examId}/Details")]
        public async Task<ActionResult<Response>> GetDetailsByExamId([FromRoute] int examId)
        {
            var result = await _examQuestionService.GetAllExamQuestionDetailsAsync(examId, true);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
