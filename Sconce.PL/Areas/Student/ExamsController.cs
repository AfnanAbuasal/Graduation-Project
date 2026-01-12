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
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        // Shows details for a specific exam.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _examService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

            // Gets all published exams for a specific section.
            [HttpGet("Section/{sectionId}")]
            public async Task<ActionResult<Response>> GetPublishedBySection([FromRoute] int sectionId)
            {
                var result = await _examService.GetPublishedBySectionIdAsync(sectionId);
                return Ok(result);
            }
    }
}
