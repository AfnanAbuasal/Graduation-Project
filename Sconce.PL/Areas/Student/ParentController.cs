using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize("Student")]
    public class ParentController : ControllerBase
    {
        private readonly IParentLinkRepository _parentLinkRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParentController(IParentLinkRepository parentLinkRepository, UserManager<ApplicationUser> userManager)
        {
            _parentLinkRepository = parentLinkRepository;
            _userManager = userManager;
        }

        //[HttpGet("ApproveLink")]
        //public async Task<IActionResult> ApproveLink(string token)
        //{
        //    var link = (await _parentLinkRepository.GetAllAsync())
        //        .FirstOrDefault(l => l.Token == token && !l.IsUsed && l.ExpiresAt > DateTime.UtcNow);

        //    if (link == null)
        //        return BadRequest("Invalid or expired token.");

        //    // Mark link as used
        //    link.IsUsed = true;
        //    link.IsApproved = true;
        //    await _parentLinkRepository.UpdateAsync(link);

        //    // Find the student and parent
        //    var parent = await _userManager.FindByIdAsync(link.ParentId);
        //    var student = await _userManager.Users
        //    .OfType<Student>()
        //    .FirstOrDefaultAsync(s => s.Email == link.StudentEmail);


        //    if (parent == null || student == null)
        //        return BadRequest("Parent or student not found.");

        //    // Create StudentParent relation
        //    var studentParent = new StudentParent
        //    {
        //        StudentId = student.Id,
        //        ParentId = parent.Id,
        //        RelationshipWithStudent = "Guardian",
        //        LinkedAt = DateTime.UtcNow,
        //        IsConfirmed = true
        //    };

        //    // Save relationship (use StudentParentRepository)
        //    await _studentParentRepository.AddAsync(studentParent);

        //    // Send confirmation emails
        //    await _notificationService.SendParentLinkApprovedAsync(student, parent);

        //    return Ok("Parent link approved successfully!");
        //}

    }
}
