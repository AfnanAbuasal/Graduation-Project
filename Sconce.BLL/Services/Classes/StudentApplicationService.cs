using Mapster;
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
    class StudentApplicationService : IStudentApplicationService
    {
        private readonly IStudentApplicationRepository _applicationRepository;
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;
        private readonly IFileUrlHelper _fileUrlHelper;

        public StudentApplicationService(
            IStudentApplicationRepository applicationRepository,
            IFileService fileService,
            INotificationService notificationService,
            IFileUrlHelper fileUrlHelper)
        {
            _applicationRepository = applicationRepository;
            _fileService = fileService;
            _notificationService = notificationService;
            _fileUrlHelper = fileUrlHelper;
        }

        public async Task<StudentApplicationResponse> SubmitApplicationAsync(StudentApplicationRequest request)
        {
            var existing = (await _applicationRepository.GetAllAsync())
                .FirstOrDefault(a => a.Email == request.Email);
            if (existing != null)
                throw new InvalidOperationException("An application with this email already exists.");

            var documentPath = await _fileService.SaveFileAsync(request.Document, "Uploads/StudentDocs");

            var application = request.Adapt<StudentApplication>();
            application.DocumentPath = documentPath;
            application.Feedback = "Your application has been submitted successfully. Please wait while it is reviewed.";

            await _applicationRepository.AddAsync(application);
            await _notificationService.SendApplicationSubmittedAsync(application);

            return application.Adapt<StudentApplicationResponse>();
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
