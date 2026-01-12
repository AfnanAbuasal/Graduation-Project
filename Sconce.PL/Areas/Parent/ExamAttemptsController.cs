using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Parent
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Parent")]
    [Authorize(Roles = "Parent")]
    public class ExamAttemptsController : ControllerBase
    {
        private readonly IExamAttemptService _examAttemptService;

        public ExamAttemptsController(IExamAttemptService examAttemptService)
        {
            _examAttemptService = examAttemptService;
        }

        // Get child's exam performance.
        [HttpPost("Performance")]
        public async Task<ActionResult<Response>> GetChildPerformance([FromBody] PerformanceFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO: Add validation to ensure parent has access to this student (their child)

            var result = await _examAttemptService.GetStudentExamPerformanceAsync(request);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
