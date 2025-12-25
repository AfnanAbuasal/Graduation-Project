using Mapster;
using Microsoft.AspNetCore.Http;
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

public class AssignmentService : FileGenericService<AssignmentRequest, AssignmentResponse, Assignment>, IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ISectionRepository _sectionRepository;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        IFileService fileService,
        IUrlHelper urlHelper,
        ISectionRepository sectionRepository)
        : base(assignmentRepository, fileService, urlHelper, "Uploads/Assignments")
    {
        _assignmentRepository = assignmentRepository;
        _sectionRepository = sectionRepository;
    }

    public async Task<Response> GetAllBySectionAsync(int sectionId, string instructorId)
    {
        // Verify section exists
        var section = await _sectionRepository.GetByIdAsync(sectionId);
        if (section == null)
            return new ErrorResponse { Errors = ["Section not found."] };

        if (section.InstructorId != instructorId)
            return new ErrorResponse { Errors = ["Unauthorized access to this section."] };
            
        // Get all assignments for this section
        var assignments = await _assignmentRepository.GetAllBySectionIdAsync(sectionId, withTracking: false);

        return new SuccessResponse<IEnumerable<AssignmentResponse>> { Data = assignments.Adapt<IEnumerable<AssignmentResponse>>() };
    }
}
