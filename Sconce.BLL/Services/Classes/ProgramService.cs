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
    public class ProgramService : GenericService<ProgramRequest, ProgramResponse, Program>, IProgramService
    {
        public ProgramService(IProgramRepository programRepository) : base(programRepository)
        {
        }
    }
}
