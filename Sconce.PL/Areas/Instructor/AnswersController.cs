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
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswersController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        // Grades an essay answer with a manual score.
        // Only available for essay questions after attempt is submitted or expired.
        [HttpPatch("{answerId}/Grade")]
        public async Task<ActionResult<Response>> GradeEssayAnswer([FromRoute] int answerId, [FromBody] GradeAnswerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _answerService.GradeEssayAnswerAsync(answerId, request.Score);

            if (!result.Success)
                return BadRequest(result.Response);

            return Ok(result.Response);
        }
    }
}
