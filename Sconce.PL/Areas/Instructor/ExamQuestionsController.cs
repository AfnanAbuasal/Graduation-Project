using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class ExamQuestionsController : ControllerBase
    {
        private readonly IExamQuestionService _examQuestionService;

        public ExamQuestionsController(IExamQuestionService examQuestionService)
        {
            _examQuestionService = examQuestionService;
        }

        // Gets all exam questions for a specific exam (basic info, no details).
        [HttpGet("Exam/{examId}")]
        public async Task<ActionResult<Response>> GetByExamId([FromRoute] int examId)
        {
            var result = await _examQuestionService.GetAllByExamIdAsync(examId);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }

        // Gets a specific exam question by ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _examQuestionService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Adds a question to an exam.
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] ExamQuestionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _examQuestionService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an exam question (change question, sort order, points, or prompt override).
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] ExamQuestionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _examQuestionService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Removes a question from an exam.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _examQuestionService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Reorders questions in an exam (bulk update of sort orders).
        [HttpPatch("Exam/{examId}/Reorder")]
        public async Task<ActionResult<Response>> Reorder(
            [FromRoute] int examId,
            [FromBody] List<ReorderItemRequest> reorderItems)
        {
            if (reorderItems == null || reorderItems.Count == 0)
                return BadRequest(new ErrorResponse { Errors = ["Reorder payload is required."] });

            var reorderList = reorderItems
                .Select(item => (item.ExamQuestionId, item.SortOrder))
                .ToList();

            var result = await _examQuestionService.ReorderAsync(examId, reorderList);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Enables or disables an exam question (Base status Active/Inactive).
        [HttpPatch("{id}/ToggleStatus")]
        public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
        {
            var result = await _examQuestionService.ToggleStatusAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Gets all exam questions with full details.
        // Includes questions and choices.
        [HttpGet("Exam/{examId}/Details")]
        public async Task<ActionResult<Response>> GetDetailsByExamId([FromRoute] int examId)
        {
            var result = await _examQuestionService.GetAllExamQuestionDetailsAsync(examId, false);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
