using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Super Admin")]
    public class ProgramsController : ControllerBase
    {
        private readonly IProgramService _programService;
        public ProgramsController(IProgramService programService)
        {
            _programService = programService;
        }

        // Lists all programs, optionally only the active ones.
        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
        {
            var programs = await _programService.GetAllAsync(onlyActive);
            return Ok(programs);
        }

        // Shows details for a specific program.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _programService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Adds a new program.
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] ProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _programService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing program.
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] ProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _programService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Removes a program.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _programService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Enables or disables a program.
        [HttpPatch("{id}/ToggleStatus")]
        public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
        {
            var result = await _programService.ToggleStatusAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Increases the planned level count for a program.
        [HttpPatch("{id}/IncreasePlannedLevelCount")]
        public async Task<ActionResult<Response>> IncreasePlannedLevelCount([FromRoute] int id, [FromBody] IncreasePlannedCountRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _programService.IncreasePlannedLevelCountAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Assigns an exam writer instructor to a program.
        [HttpPatch("{programId}/ProficiencyExam/Writer")]
        public async Task<ActionResult<Response>> AssignExamWriterInstructor([FromRoute] int programId, [FromBody] AssignProgramInstructorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _programService.AssignExamWriterInstructorAsync(programId, request.InstructorId);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Assigns an evaluator instructor to a program.
        [HttpPatch("{programId}/ProficiencyExam/Evaluator")]
        public async Task<ActionResult<Response>> AssignEvaluatorInstructor([FromRoute] int programId, [FromBody] AssignProgramInstructorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _programService.AssignEvaluatorInstructorAsync(programId, request.InstructorId);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
