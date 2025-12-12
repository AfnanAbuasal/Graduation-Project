using Mapster;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
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
    private readonly IFileService _fileService;
    private readonly IUrlHelper _urlHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IFileService fileService,
        IUrlHelper urlHelper,
        IAssignmentRepository assignmentRepository,
        IHttpContextAccessor httpContextAccessor)
        : base(submissionRepository, fileService, urlHelper, "Uploads/Submissions")
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _fileService = fileService;
        _urlHelper = urlHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(SubmissionRequest request)
    {
        // Get StudentId from JWT claims
        var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(studentId))
            return (0, new ErrorResponse { Errors = ["User not authenticated."] });

        // Validate that the assignment exists and is not past the due date
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId);

        if (assignment == null)
            return (0, new ErrorResponse { Errors = ["Assignment not found."] });

        if (DateTime.UtcNow > assignment.DueDate)
            return (0, new ErrorResponse { Errors = ["Assignment submission deadline has passed."] });

        // Create submission with StudentId from JWT
        var submission = request.Adapt<Submission>();
        submission.StudentId = studentId;

        if (request.File != null)
            submission.FilePath = await _fileService.SaveFileAsync(request.File, "Uploads/Submissions");

        var rows = await _submissionRepository.AddAsync(submission);

        return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
    }

    public override async Task<Response> GetAllAsync(bool onlyActive = false)
    {
        var list = await _submissionRepository.GetAllWithStudentAsync();

        if (onlyActive)
            list = list.Where(x => x.Status == Sconce.DAL.Models.Enums.Status.Active);

        var responseList = new List<SubmissionResponse>();

        foreach (var entity in list)
        {
            var dto = entity.Adapt<SubmissionResponse>();
            dto.FileUrl = _urlHelper.BuildUrl(entity.FilePath);
            dto.StudentName = entity.Student?.FullName;

            responseList.Add(dto);
        }

        return new SuccessResponse<IEnumerable<SubmissionResponse>> { Data = responseList };
    }

    public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, SubmissionRequest request)
    {
        var submission = await _submissionRepository.GetByIdAsync(id);

        if (submission == null)
            return (0, new ErrorResponse { Errors = ["Submission not found."] });

        // Get StudentId from JWT claims and verify ownership
        var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(studentId) || submission.StudentId != studentId)
            return (0, new ErrorResponse { Errors = ["Not authorized to update this submission."] });

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
