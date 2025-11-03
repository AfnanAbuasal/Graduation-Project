using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    public class AdminInstructorService : IAdminInstructorService
    {
        private readonly IInstructorApplicationRepository _applicationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INotificationService _notificationService;
        private readonly IFileUrlHelper _fileUrlHelper;

        public AdminInstructorService(
            IInstructorApplicationRepository applicationRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            INotificationService notificationService,
            IFileUrlHelper fileUrlHelper)
        {
            _applicationRepository = applicationRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationService = notificationService;
            _fileUrlHelper = fileUrlHelper;
        }

        public async Task<IEnumerable<InstructorApplicationResponse>> GetAllApplicationsAsync(ApplicationStatus? status = null)
        {
            var apps = await _applicationRepository.GetAllAsync();
            if (status.HasValue)
                apps = apps.Where(a => a.ApplicationStatus == status.Value);

            var responses = apps.Adapt<IEnumerable<InstructorApplicationResponse>>().ToList();
            foreach (var res in responses)
            {
                res.CVUrl = _fileUrlHelper.BuildFileUrl(res.CVPath);
            }

            return responses;
        }

        public async Task<InstructorApplicationResponse?> GetApplicationByIdAsync(int id)
        {
            var app = await _applicationRepository.GetByIdAsync(id);
            if (app == null) return null;

            var response = app.Adapt<InstructorApplicationResponse>();
            response.CVUrl = _fileUrlHelper.BuildFileUrl(response.CVPath);

            return response;
        }

        public async Task<bool> ReviewApplicationAsync(int id, ApplicationStatus newStatus, string feedback)
        {
            var app = await _applicationRepository.GetByIdAsync(id);
            if (app == null)
                return false;

            if (app.ApplicationStatus == ApplicationStatus.Approved || app.ApplicationStatus == ApplicationStatus.Rejected)
                throw new InvalidOperationException("Only pending applications can be reviewed.");

            app.ApplicationStatus = newStatus;
            app.Feedback = feedback;

            await _applicationRepository.UpdateAsync(app);

            if (newStatus == ApplicationStatus.Approved)
            {
                var existingUser = await _userManager.FindByEmailAsync(app.Email);
                if (existingUser != null) return true; // user already exists (safety)

                var instructorUser = new Instructor
                {
                    UserName = app.Email.Split('@')[0],
                    Email = app.Email,
                    FullName = app.FullName,
                    Gender = app.Gender,
                    Country = app.Country,
                    City = app.City,
                    Street = app.Street,
                    PhoneNumber = app.PhoneNumber,
                    DateOfBirth = app.DateOfBirth
                };

                // Create personalized default password
                var cleanName = new string(app.FullName
                    .Where(c => char.IsLetterOrDigit(c))
                    .ToArray());
                var capitalized = char.ToUpper(cleanName[0]) + cleanName.Substring(1);
                var defaultPassword = $"{capitalized}@123";

                var result = await _userManager.CreateAsync(instructorUser, defaultPassword);
                if (!result.Succeeded)
                    return false;

                // Ensure role exists
                if (!await _roleManager.RoleExistsAsync("Instructor"))
                    await _roleManager.CreateAsync(new IdentityRole("Instructor"));

                await _userManager.AddToRoleAsync(instructorUser, "Instructor");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(instructorUser);
                var escapedToken = Uri.EscapeDataString(token);

                var confirmationRelativePath = $"/api/Identity/Account/ConfirmEmail?token={escapedToken}&userID={instructorUser.Id}";
                var emailConfirmationURL = _fileUrlHelper.BuildFileUrl(confirmationRelativePath);

                await _notificationService.SendApplicationApprovedAsync(app, defaultPassword, emailConfirmationURL);

            }
            else if (newStatus == ApplicationStatus.Rejected)
            {
                await _notificationService.SendApplicationRejectedAsync(app);
            }
            return true;
        }
    }
}
