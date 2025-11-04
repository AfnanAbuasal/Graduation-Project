using Mapster;
using Microsoft.AspNetCore.Identity;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class StudentApplicationService : IStudentApplicationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentApplicationRepository _applicationRepository;
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;
        private readonly IFileUrlHelper _fileUrlHelper;

        public StudentApplicationService(
            UserManager<ApplicationUser> userManager,
            IStudentApplicationRepository applicationRepository,
            IFileService fileService,
            INotificationService notificationService,
            IFileUrlHelper fileUrlHelper)
        {
            _userManager = userManager;
            _applicationRepository = applicationRepository;
            _fileService = fileService;
            _notificationService = notificationService;
            _fileUrlHelper = fileUrlHelper;
        }

        public async Task<StudentApplicationResponse> SubmitApplicationAsync(StudentApplicationRequest request)
        {
            // 1. Ensure a registered user exists
            var studentUser = await _userManager.FindByEmailAsync(request.Email);
            if (studentUser == null)
                throw new InvalidOperationException("No student account found with this email.");

            // 2. Prevent duplicate applications
            var existing = (await _applicationRepository.GetAllAsync())
                .FirstOrDefault(a => a.Email == request.Email);
            if (existing != null)
                throw new InvalidOperationException("An application with this email already exists.");

            // 3. Save uploaded document
            var documentPath = await _fileService.SaveFileAsync(request.Document, "Uploads/StudentDocs");

            // 4. Map and assign from both request + student user
            var application = request.Adapt<StudentApplication>();
            application.FullName = studentUser.FullName;
            application.Email = studentUser.Email;
            application.DocumentPath = documentPath;
            application.Feedback = "Your student application has been submitted successfully. Please wait while it is reviewed.";

            // 5. Save to DB
            await _applicationRepository.AddAsync(application);

            // 6. Notify student (optional)
            await _notificationService.SendApplicationSubmittedAsync(application);

            // 7. Map response
            var response = application.Adapt<StudentApplicationResponse>();
            response.DocumentUrl = _fileUrlHelper.BuildFileUrl(application.DocumentPath);

            return response;
        }

        public async Task<StudentApplicationResponse?> GetApplicationByEmailAsync(string email)
        {
            var app = (await _applicationRepository.GetAllAsync())
                .FirstOrDefault(a => a.Email == email);

            if (app == null) return null;

            var response = app.Adapt<StudentApplicationResponse>();
            response.DocumentUrl = _fileUrlHelper.BuildFileUrl(app.DocumentPath);
            return response;
        }
    }
}
