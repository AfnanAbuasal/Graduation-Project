using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Classes;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes;

public class AssignmentService : FileGenericService<AssignmentRequest, AssignmentResponse, Assignment> ,IAssignmentService
{
    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        IFileService fileService,
        IUrlHelper urlHelper)
        : base(assignmentRepository, fileService, urlHelper, "Uploads/Assignments")
    { }
}
