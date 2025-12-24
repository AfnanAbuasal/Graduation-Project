using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.PL.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Super Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public UsersController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        // Lists all user profiles, optionally filtered by user type.
        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] UserType? type = null)
        {
            var result = await _adminUserService.GetAllUserProfilesAsync(type);
            return Ok(result);
        }

        // Shows details for a specific user profile.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] string id)
        {
            var result = await _adminUserService.GetUserProfileByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Deletes a student user by ID, optionally including their application.
        [HttpDelete("students/{id}")]
        public async Task<ActionResult<Response>> DeleteStudent([FromRoute] string id, [FromQuery] bool deleteApplication = true)
        {
            var result = await _adminUserService.DeleteStudentByIdAsync(id, deleteApplication);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Deletes an instructor user by ID, optionally including their application.
        [HttpDelete("instructors/{id}")]
        public async Task<ActionResult<Response>> DeleteInstructor([FromRoute] string id, [FromQuery] bool deleteApplication = true)
        {
            var result = await _adminUserService.DeleteInstructorByIdAsync(id, deleteApplication);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Deletes a parent user by ID and all associated student links.
        [HttpDelete("parents/{id}")]
        public async Task<ActionResult<Response>> DeleteParent([FromRoute] string id)
        {
            var result = await _adminUserService.DeleteParentByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
