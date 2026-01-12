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
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        // Get child's assignment performance.
        [HttpPost("Performance")]
        public async Task<ActionResult<Response>> GetChildPerformance([FromBody] PerformanceFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO: Add validation to ensure parent has access to this student (their child)
            // This would require checking StudentParent relationship

            var result = await _assignmentService.GetStudentAssignmentPerformanceAsync(request);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
