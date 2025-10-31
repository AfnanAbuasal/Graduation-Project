using Mapster;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    public class InstructorApplicationService : IInstructorApplicationService
    {
        private readonly IInstructorApplicationRepository _applicationRepository;
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;

        public InstructorApplicationService(IInstructorApplicationRepository applicationRepository, IFileService fileService, INotificationService notificationService)
        {
            _applicationRepository = applicationRepository;
            _fileService = fileService;
            _notificationService = notificationService;
        }

        public async Task<InstructorApplicationResponse> SubmitApplicationAsync(InstructorApplicationRequest request)
        {
            var existing = (await _applicationRepository.GetAllAsync())
                .FirstOrDefault(a => a.Email == request.Email);

            if (existing != null)
                throw new InvalidOperationException("An application with this email already exists.");

            var cvPath = await _fileService.SaveFileAsync(request.CV, "Uploads/CVs");

            var application = request.Adapt<InstructorApplication>();
            application.CVPath = cvPath;
            application.Feedback = "Your instructor application has been submitted successfully. Please wait while the manager reviews it.";

            await _applicationRepository.AddAsync(application);

            await _notificationService.SendApplicationSubmittedAsync(application);

            return application.Adapt<InstructorApplicationResponse>();
        }
        public async Task<InstructorApplicationResponse?> GetApplicationByEmailAsync(string email)
        {
            var app = (await _applicationRepository.GetAllAsync())
                            .FirstOrDefault(a => a.Email == email);

            return app?.Adapt<InstructorApplicationResponse>();
        }
    }
}
