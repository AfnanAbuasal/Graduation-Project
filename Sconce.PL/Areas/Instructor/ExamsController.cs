using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;
using System.Threading.Tasks;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        // Lists all exams, optionally only the active ones.
        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
        {
            var result = await _examService.GetAllAsync(onlyActive);
            return Ok(result);
        }

        // Shows details for a specific exam.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _examService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Gets all exams for a section.
        [HttpGet("Section/{sectionId}")]
        public async Task<ActionResult<Response>> GetBySection([FromRoute] int sectionId, [FromQuery] bool onlyActive = false)
        {
            var result = await _examService.GetAllBySectionAsync(sectionId, onlyActive);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }

        // Creates a new exam.
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] ExamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _examService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing exam.
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] ExamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _examService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Deletes an exam.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _examService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Changes the workflow status of an exam (Draft -> Published -> Closed).
        [HttpPatch("{id}/ExamStatus")]
        public async Task<ActionResult<Response>> ChangeStatus([FromRoute] int id, [FromQuery] ExamStatus newStatus)
        {
            var result = await _examService.ChangeExamStatusAsync(id, newStatus);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Enables or disables an exam (Base status Active/Inactive).
        [HttpPatch("{id}/ToggleStatus")]
        public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
        {
            var result = await _examService.ToggleStatusAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
