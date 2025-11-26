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
        private readonly IUrlHelper _urlHelper;

        public StudentApplicationService(
            UserManager<ApplicationUser> userManager,
            IStudentApplicationRepository applicationRepository,
            IFileService fileService,
            INotificationService notificationService,
            IUrlHelper urlHelper)
        {
            _userManager = userManager;
            _applicationRepository = applicationRepository;
            _fileService = fileService;
            _notificationService = notificationService;
            _urlHelper = urlHelper;
        }

        public async Task<Response> SubmitApplicationAsync(StudentApplicationRequest request)
        {
            // Ensure a registered user exists
            var studentUser = await _userManager.FindByEmailAsync(request.Email);
            if (studentUser == null)
                return new Response { Message = "No student account found with this email." };

            // Prevent duplicate applications
            var existing = (await _applicationRepository.GetAllAsync())
                .FirstOrDefault(a => a.Email == request.Email);
            if (existing != null)
                return new Response { Message = "An application with this email already exists." };

            // Save uploaded document
            var documentPath = await _fileService.SaveFileAsync(request.Document, "Uploads/StudentDocs");

            // Map and assign from both request + student user
            var application = request.Adapt<StudentApplication>();
            application.FullName = studentUser.FullName;
            application.Email = studentUser.Email;
            application.DocumentPath = documentPath;
            application.Feedback = "Your student application has been submitted successfully. Please wait while it is reviewed.";

            // Save to DB
            await _applicationRepository.AddAsync(application);

            // Notify student (optional)
            await _notificationService.SendApplicationSubmittedAsync(application);

            // Map response
            var response = application.Adapt<StudentApplicationResponse>();
            response.DocumentUrl = _urlHelper.BuildUrl(application.DocumentPath);

            return response;
        }

        public async Task<Response> GetApplicationByEmailAsync(string email)
        {
            var app = (await _applicationRepository.GetAllAsync())
                .FirstOrDefault(a => a.Email == email);

            if (app == null) 
                return new Response { Message = "No application found for this email."};

            var response = app.Adapt<StudentApplicationResponse>();
            response.DocumentUrl = _urlHelper.BuildUrl(app.DocumentPath);

            return response;
        }
    }
}
