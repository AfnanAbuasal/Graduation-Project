using Mapster;
using Microsoft.AspNetCore.Http;
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
using static System.Net.Mime.MediaTypeNames;

namespace Sconce.BLL.Services.Classes
{
    public class InstructorApplicationService : IInstructorApplicationService
    {
        private readonly IInstructorApplicationRepository _applicationRepository;
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUrlHelper _urlHelper;

        public InstructorApplicationService(IInstructorApplicationRepository applicationRepository,
            IFileService fileService,
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager,
            IUrlHelper urlHelper)
        {
            _applicationRepository = applicationRepository;
            _fileService = fileService;
            _notificationService = notificationService;
            _userManager = userManager;
            _urlHelper = urlHelper;
        }

        public async Task<(bool Success, Response Response)> SubmitApplicationAsync(InstructorApplicationRequest request)
        {
            if(await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return (false, new Response
                {
                    Message = "An account with this email already exists."
                });
            }
            
            if ((await _applicationRepository.GetAllAsync())
                .FirstOrDefault(a => a.Email == request.Email) != null)
            {
                return (false, new Response
                {
                    Message = "An application with this email already exists."
                });
            }

            var cvPath = await _fileService.SaveFileAsync(request.CV, "Uploads/CVs");

            var app = request.Adapt<InstructorApplication>();
            app.CVPath = cvPath;
            app.Feedback = "Your instructor application has been submitted successfully. Please wait while the manager reviews it.";

            await _applicationRepository.AddAsync(app);

            await _notificationService.SendApplicationSubmittedAsync(app);

            var response = app.Adapt<InstructorApplicationResponse>();
            response.CVUrl = _urlHelper.BuildUrl(app.CVPath);
            response.Message = "";

            return (true, response);
        }
        public async Task<(bool Success, Response Response)> GetApplicationByEmailAsync(string email)
        {
            var app = (await _applicationRepository.GetAllAsync())
                    .FirstOrDefault(a => a.Email == email);

            if (app == null)
            { 
                return (false, new Response { Message = "No application found for this email." });
            }

            var response = app.Adapt<InstructorApplicationResponse>();
            response.CVUrl = _urlHelper.BuildUrl(app.CVPath);
            response.Message = "";

            return (true, response);
        }
    }
}
