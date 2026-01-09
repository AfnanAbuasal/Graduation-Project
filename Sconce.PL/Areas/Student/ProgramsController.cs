using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class ProgramsController : ControllerBase
    {
        private readonly IProgramService _programService;

        public ProgramsController(IProgramService programService)
        {
            _programService = programService;
        }

        // Get all programs that the authenticated student is enrolled in.
        // returns List of programs the student is enrolled in
        [HttpGet("Enrolled")]
        public async Task<ActionResult<Response>> GetEnrolledPrograms()
        {
            var response = await _programService.GetProgramsForStudentAsync();

            if (response is ErrorResponse)
                return BadRequest(response);

            return Ok(response);
        }

        // Get the proficiency exam for a specific program.
        // returns The proficiency exam for the program
        [HttpGet("{programId}/ProficiencyExam")]
        public async Task<ActionResult<Response>> GetProficiencyExam([FromRoute] int programId)
        {
            var response = await _programService.GetProficiencyExamForProgramAsync(programId);

            if (response is ErrorResponse)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
