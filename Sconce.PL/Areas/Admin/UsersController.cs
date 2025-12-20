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
    }
}
