using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class DashboardStatsResponse
    {
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int ActiveCourses { get; set; }
    }
}
