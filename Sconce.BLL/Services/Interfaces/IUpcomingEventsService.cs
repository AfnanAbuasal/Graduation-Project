using Sconce.DAL.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IUpcomingEventsService
    {
        Task<Response> GetStudentUpcomingEventsAsync(string studentId, int? windowDays);
    }
}
