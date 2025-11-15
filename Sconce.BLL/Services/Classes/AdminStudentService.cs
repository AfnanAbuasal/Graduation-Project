using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class AdminStudentService : IAdminStudentService
    {
        private readonly IStudentApplicationRepository _applicationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IUrlHelper _urlHelper;
        private readonly IParentInviteRepository _parentInviteRepository;

        public AdminStudentService(
            IStudentApplicationRepository applicationRepository,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            IUrlHelper urlHelper,
            IParentInviteRepository parentInviteRepository)
        {
            _applicationRepository = applicationRepository;
            _userManager = userManager;
            _notificationService = notificationService;
            _urlHelper = urlHelper;
            _parentInviteRepository = parentInviteRepository;
        }

        public async Task<IEnumerable<StudentApplicationResponse>> GetAllApplicationsAsync(ApplicationStatus? status = null)
        {
            var apps = await _applicationRepository.GetAllAsync();
            if (status.HasValue)
                apps = apps.Where(a => a.ApplicationStatus == status.Value);

            var responses = apps.Adapt<IEnumerable<StudentApplicationResponse>>().ToList();
            foreach (var res in responses)
                res.DocumentUrl = _urlHelper.BuildUrl(res.DocumentPath);

            return responses;
        }

        public async Task<StudentApplicationResponse?> GetApplicationByIdAsync(int id)
        {
            var app = await _applicationRepository.GetByIdAsync(id);
            if (app == null) return null;

            var response = app.Adapt<StudentApplicationResponse>();
            response.DocumentUrl = _urlHelper.BuildUrl(response.DocumentPath);

            return response;
        }

        public async Task<bool> ReviewApplicationAsync(int id, ApplicationStatus newStatus, string feedback)
        {
            var app = await _applicationRepository.GetByIdAsync(id);
            if (app == null)
                return false;

            if (app.ApplicationStatus != ApplicationStatus.Pending)
                throw new InvalidOperationException("Only pending applications can be reviewed.");

            app.ApplicationStatus = newStatus;
            app.Feedback = feedback;

            await _applicationRepository.UpdateAsync(app);

            if (newStatus == ApplicationStatus.Approved)
            {
                var studentUser = await _userManager.Users
                    .OfType<Student>()
                    .FirstOrDefaultAsync(s => s.Email == app.Email);
                if (studentUser == null)
                    throw new InvalidOperationException("Student user not found.");

                // Move application data to Student account
                studentUser.DateOfBirth = app.DateOfBirth;
                studentUser.Gender = app.Gender;
                studentUser.DocumentPath = app.DocumentPath;
                studentUser.LevelOfProficiency = app.LevelOfProficiency;

                // Update student user in Identity
                var result = await _userManager.UpdateAsync(studentUser);
                if (!result.Succeeded)
                    throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

                // Notify student of approval
                await _notificationService.SendApplicationApprovedAsync(app);

                // If guardian info exists, generate and send invite
                if (!string.IsNullOrEmpty(app.GuardianEmail))
                {
                    var token = Guid.NewGuid().ToString("N");

                    var invite = new ParentInvite
                    {
                        Token = token,
                        StudentId = studentUser.Id,
                        GuardianEmail = app.GuardianEmail!,
                        ExpiresAt = DateTime.UtcNow.AddDays(3),
                        IsUsed = false
                    };

                    await _parentInviteRepository.AddAsync(invite);

                    // Frontend registration link for parent (to be replaced with actual URL)
                    var frontendUrl = $"https://sconce-frontend.com/register/parent?token={token}";

                    await _notificationService.SendParentInvitationAsync(app, frontendUrl);
                }
            }
            else if (newStatus == ApplicationStatus.Rejected)
            {
                await _notificationService.SendApplicationRejectedAsync(app);
            }

            return true;
        }
    }
}
