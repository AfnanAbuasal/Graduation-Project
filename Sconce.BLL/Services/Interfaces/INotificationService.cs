using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendApplicationSubmittedAsync(InstructorApplication app);
        Task SendApplicationApprovedAsync(InstructorApplication app, string? password = null);
        Task SendApplicationRejectedAsync(InstructorApplication app);
    }
}
