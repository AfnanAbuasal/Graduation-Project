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
    public class ProgramsController : ControllerBase
    {
        private readonly IProgramService _programService;

        public ProgramsController(IProgramService programService)
        {
            _programService = programService;
        }

        // Gets all programs where the current instructor is assigned as Proficiency Exam Writer.
        [HttpGet("ProficiencyExamWriter")]
        public async Task<ActionResult<Response>> GetProficiencyExamWriterPrograms()
        {
            var result = await _programService.GetProgramsForExamWriterAsync();
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }

        // Gets all programs where the current instructor is assigned as Proficiency Exam Evaluator.
        [HttpGet("ProficiencyExamEvaluator")]
        public async Task<ActionResult<Response>> GetProficiencyExamEvaluatorPrograms()
        {
            var result = await _programService.GetProgramsForEvaluatorAsync();
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
