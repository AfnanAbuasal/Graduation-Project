using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Super Admin")]
    public class ProgramEnrollmentsController : ControllerBase
    {
        private readonly IProgramEnrollmentService _programEnrollmentService;

        public ProgramEnrollmentsController(IProgramEnrollmentService programEnrollmentService)
        {
            _programEnrollmentService = programEnrollmentService;
        }

        // Get enrollments for a program with filtering and sorting
        [HttpGet("Program/{programId}")]
        public async Task<ActionResult<Response>> GetEnrollments(
            [FromRoute] int programId,
            [FromQuery] string? placementStatus = null,
            [FromQuery] string? examStatus = null,
            [FromQuery] int? recommendedCourseId = null,
            [FromQuery] string sortOrder = "oldest",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Normalize and validate inputs
            placementStatus = placementStatus?.Trim().ToLower();
            examStatus = examStatus?.Trim().ToLower();
            sortOrder = sortOrder?.Trim().ToLower() ?? "oldest";

            // Validate sort order
            if (!new[] { "newest", "oldest" }.Contains(sortOrder))
                return BadRequest("Invalid sort order. Use 'newest' or 'oldest'.");

            // Validate placement status
            if (!string.IsNullOrEmpty(placementStatus) && !new[] { "placed", "notplaced" }.Contains(placementStatus))
                return BadRequest("Invalid placement status. Use 'placed' or 'notplaced'.");
            
            // Validate exam status
            if (!string.IsNullOrEmpty(examStatus) && !new[] { "inprogress", "submitted", "graded", "nottaken" }.Contains(examStatus))
                return BadRequest("Invalid exam status. Use 'inprogress', 'submitted', 'graded', or 'nottaken'.");

            // Validate pagination
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Page number and page size must be greater than 0.");
            
            var (enrollments, totalCount) = await _programEnrollmentService.GetEnrollmentsForProgramAsync(
                programId,
                placementStatus,
                examStatus,
                recommendedCourseId,
                sortOrder,
                pageNumber,
                pageSize);

            return Ok(new
            {
                data = enrollments,
                totalCount,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
    }
}
