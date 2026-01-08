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
    public class ProgramsController : ControllerBase
    {
        private readonly IProgramService _programService;
        private readonly IProgramEnrollmentService _programEnrollmentService;

        public ProgramsController(IProgramService programService, IProgramEnrollmentService programEnrollmentService)
        {
            _programService = programService;
            _programEnrollmentService = programEnrollmentService;
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

        // Sets the recommended course for a student's finalized proficiency exam attempt.
        [HttpPut("{programId}/Students/{studentId}/RecommendedCourse")]
        public async Task<ActionResult<Response>> SetRecommendedCourse([FromRoute] int programId, [FromRoute] string studentId, [FromBody] RecommendCourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _programEnrollmentService.SetRecommendedCourseAsync(programId, studentId, request.RecommendedCourseId);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
