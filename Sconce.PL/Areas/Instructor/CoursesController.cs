using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Instructor;

[Route("api/[area]/[controller]")]
[ApiController]
[Area("Instructor")]
[Authorize(Roles = "Instructor")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }
    
    // Gets all courses in a program for the instructor, ordered by level prerequisites then course order.
    [HttpGet("{programId}/Courses")]
    public async Task<ActionResult<Response>> GetCoursesByProgram([FromRoute] int programId, [FromQuery] bool onlyActive = false)
    {
        var result = await _courseService.GetByProgramAsync(programId, onlyActive);
        return Ok(result);
    }
}
