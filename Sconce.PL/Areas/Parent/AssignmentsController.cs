using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Parent
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Parent")]
    [Authorize(Roles = "Parent")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IParentAccessService _parentAccessService;

        public AssignmentsController(IAssignmentService assignmentService, IParentAccessService parentAccessService)
        {
            _assignmentService = assignmentService;
            _parentAccessService = parentAccessService;
        }

        // Get child's assignment performance.
        [HttpPost("Performance")]
        public async Task<ActionResult<Response>> GetChildPerformance([FromBody] PerformanceFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(parentId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            // Validate parent has access to this student
            var (hasAccess, errorMessage) = await _parentAccessService.ValidateParentAccessToStudentAsync(parentId, request.StudentId);
            if (!hasAccess)
                return Forbid(errorMessage);

            var result = await _assignmentService.GetStudentAssignmentPerformanceAsync(request);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
