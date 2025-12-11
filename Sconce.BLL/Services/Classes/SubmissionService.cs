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

namespace Sconce.BLL.Services.Classes;

public class SubmissionService : FileGenericService<SubmissionRequest, SubmissionResponse, Submission>, ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IFileService fileService,
        IUrlHelper urlHelper,
        IAssignmentRepository assignmentRepository)
        : base(submissionRepository, fileService, urlHelper, "Uploads/Submissions")
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
    }

    public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(SubmissionRequest request)
    {
        // Validate that the assignment exists and is not past the due date
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId);

        if (assignment == null)
            return (0, new ErrorResponse { Errors = ["Assignment not found."] });

        if (DateTime.UtcNow > assignment.DueDate)
            return (0, new ErrorResponse { Errors = ["Assignment submission deadline has passed."] });

        // If validation passes, proceed with the base implementation
        return await base.CreateAsync(request);
    }

    public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, SubmissionRequest request)
    {
        var submission = await _submissionRepository.GetByIdAsync(id);

        if (submission == null)
            return (0, new ErrorResponse { Errors = ["Submission not found."] });

        // Validate that the assignment is not past the due date
        var assignment = await _assignmentRepository.GetByIdAsync(submission.AssignmentId);

        if (assignment == null)
            return (0, new ErrorResponse { Errors = ["Assignment not found."] });

        if (DateTime.UtcNow > assignment.DueDate)
            return (0, new ErrorResponse { Errors = ["Assignment submission deadline has passed. Cannot update submission."] });

        // If validation passes, proceed with the base implementation
        return await base.UpdateAsync(id, request);
    }

    public async Task<(bool Success, Response Response)> GradeSubmissionAsync(int submissionId, GradeSubmissionRequest request)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);

        if (submission == null)
            return (false, new ErrorResponse { Errors = ["Assignment Submission not found."] });

        submission.Grade = request.Grade;
        submission.Feedback = request.Feedback;
        submission.GradedAt = DateTime.UtcNow;

        await _submissionRepository.UpdateAsync(submission);

        return (true, new SuccessResponse<string> { Data = "AssignmentSubmission graded successfully." });
    }
}
