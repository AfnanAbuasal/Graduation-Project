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
using Sconce.DAL.Models.Enums;

namespace Sconce.BLL.Services.Classes;

public class SubmissionService : FileGenericService<SubmissionRequest, SubmissionResponse, Submission>, ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IFileService _fileService;
    private readonly IUrlHelper _urlHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationService;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IFileService fileService,
        IUrlHelper urlHelper,
        IAssignmentRepository assignmentRepository,
        IHttpContextAccessor httpContextAccessor,
        INotificationService notificationService)
        : base(submissionRepository, fileService, urlHelper, "Uploads/Submissions")
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _fileService = fileService;
        _urlHelper = urlHelper;
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
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

        if (rows > 0)
        {
            var submissionWithStudent = await _submissionRepository.GetByIdWithStudentAsync(submission.Id);

            if (submissionWithStudent != null)
                await _notificationService.SendSubmissionCreatedAsync(submissionWithStudent, assignment);
        }

        return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
    }

    public override async Task<(bool Success, Response Response)> GetByIdAsync(int id)
    {
        var submission = await _submissionRepository.GetByIdWithStudentAsync(id);

        if (submission == null)
            return (false, new ErrorResponse { Errors = ["Not Found."] });

        var dto = submission.Adapt<SubmissionResponse>();
        dto.FileUrl = _urlHelper.BuildUrl(submission.FilePath);
        dto.StudentName = submission.Student?.FullName;

        return (true, new SuccessResponse<SubmissionResponse> { Data = dto });
    }

    public override async Task<Response> GetAllAsync(bool onlyActive = false)
    {
        var list = await _submissionRepository.GetAllWithStudentAsync();

        if (onlyActive)
            list = list.Where(x => x.Status == Status.Active);

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

    public async Task<(bool Success, Response Response)> GetMySubmissionByAssignmentAsync(int assignmentId)
    {
        var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(studentId))
            return (false, new ErrorResponse { Errors = ["User not authenticated."] });

        var submission = await _submissionRepository.GetByAssignmentAndStudentAsync(assignmentId, studentId);

        if (submission == null)
            return (false, new ErrorResponse { Errors = ["Submission not found."] });

        var dto = submission.Adapt<SubmissionResponse>();
        dto.FileUrl = _urlHelper.BuildUrl(submission.FilePath);
        dto.StudentName = submission.Student?.FullName;

        return (true, new SuccessResponse<SubmissionResponse> { Data = dto });
    }

    public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id)
    {
        var submission = await _submissionRepository.GetByIdWithStudentAsync(id);

        if (submission == null)
            return (0, new ErrorResponse { Errors = ["Submission not found."] });

        var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(studentId) || submission.StudentId != studentId)
            return (0, new ErrorResponse { Errors = ["Not authorized to delete this submission."] });

        var assignment = await _assignmentRepository.GetByIdAsync(submission.AssignmentId);

        if (assignment == null)
            return (0, new ErrorResponse { Errors = ["Assignment not found."] });

        if (DateTime.UtcNow > assignment.DueDate)
            return (0, new ErrorResponse { Errors = ["Assignment submission deadline has passed. Cannot delete submission."] });

        var (rows, response) = await base.DeleteAsync(id);

        if (rows > 0)
            await _notificationService.SendSubmissionDeletedAsync(submission, assignment);

        return (rows, response);
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

        request.Adapt(submission);

        if (request.File != null)
        {
            if (!string.IsNullOrEmpty(submission.FilePath))
                _fileService.DeleteFileAsync(submission.FilePath);

            submission.FilePath = await _fileService.SaveFileAsync(request.File, "Uploads/Submissions");
        }

        submission.UpdatedAt = DateTime.UtcNow;

        var rows = await _submissionRepository.UpdateAsync(submission);

        if (rows > 0)
        {
            var submissionWithStudent = await _submissionRepository.GetByIdWithStudentAsync(id);

            if (submissionWithStudent != null)
                await _notificationService.SendSubmissionUpdatedAsync(submissionWithStudent, assignment);
        }

        return (rows, new SuccessResponse<string>
        {
            Data = $"{rows} record(s) updated successfully."
        });
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

        var submissionWithStudent = await _submissionRepository.GetByIdWithStudentAsync(submissionId);
        var assignment = await _assignmentRepository.GetByIdAsync(submission.AssignmentId);

        if (submissionWithStudent != null && assignment != null)
            await _notificationService.SendSubmissionGradedAsync(submissionWithStudent, assignment);

        return (true, new SuccessResponse<string> { Data = "AssignmentSubmission graded successfully." });
    }
}
