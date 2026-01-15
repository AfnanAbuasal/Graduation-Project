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
    private readonly ISubmissionRepository _submissionRepository;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        IFileService fileService,
        IUrlHelper urlHelper,
        ISectionRepository sectionRepository,
        ISubmissionRepository submissionRepository)
        : base(assignmentRepository, fileService, urlHelper, "Uploads/Assignments")
    {
        _assignmentRepository = assignmentRepository;
        _sectionRepository = sectionRepository;
        _submissionRepository = submissionRepository;
    }

    public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(AssignmentRequest request)
    {
        var result = await base.CreateAsync(request);
        
        if (result.NumberOfEntries > 0)
        {
            await UpdateSectionTimestampAsync(request.SectionId);
        }
        
        return result;
    }

    public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, AssignmentRequest request)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null)
            return (0, new ErrorResponse { Errors = ["Not Found."] });

        var sectionId = assignment.SectionId;
        var result = await base.UpdateAsync(id, request);
        
        if (result.NumberOfEntries > 0 && sectionId.HasValue)
        {
            await UpdateSectionTimestampAsync(sectionId.Value);
        }
        
        return result;
    }

    public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null)
            return (0, new ErrorResponse { Errors = ["Not Found."] });

        var sectionId = assignment.SectionId;
        var result = await base.DeleteAsync(id);
        
        if (result.NumberOfEntries > 0 && sectionId.HasValue)
        {
            await UpdateSectionTimestampAsync(sectionId.Value);
        }
        
        return result;
    }

    private async Task UpdateSectionTimestampAsync(int sectionId)
    {
        var section = await _sectionRepository.GetByIdAsync(sectionId);
        if (section != null)
        {
            section.UpdatedAt = DateTime.UtcNow;
            await _sectionRepository.UpdateAsync(section);
        }
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

    public async Task<Response> GetStudentAssignmentPerformanceAsync(PerformanceFilterRequest request)
    {
        // Verify section exists
        var section = await _sectionRepository.GetByIdAsync(request.SectionId);
        if (section == null)
            return new ErrorResponse { Errors = ["Section not found."] };

        // Verify student exists and is enrolled in the section
        var studentSections = await _sectionRepository.GetStudentSectionsAsync(request.StudentId);
        var studentSection = studentSections.FirstOrDefault(ss => ss.SectionId == request.SectionId);
        if (studentSection == null)
            return new ErrorResponse { Errors = ["Student is not enrolled in this section."] };

        // Calculate time window
        DateTime windowStart;
        if (request.WindowDays.HasValue)
        {
            windowStart = DateTime.UtcNow.AddDays(-request.WindowDays.Value);
        }
        else
        {
            // Use section creation date as start
            windowStart = section.CreatedAt;
        }

        // Get all assignments in the section within the time window (past assignments only - due date before now)
        var allAssignments = await _assignmentRepository.GetAllBySectionIdAsync(request.SectionId, withTracking: false);
        var pastAssignments = allAssignments
            .Where(a => a.DueDate >= windowStart && a.DueDate <= DateTime.UtcNow)
            .OrderBy(a => a.DueDate)
            .ToList();

        // Get all submissions for this student in this section
        var allSubmissions = await _submissionRepository.GetAllAsync(withTracking: false);
        var studentSubmissions = allSubmissions
            .Where(s => s.StudentId == request.StudentId && 
                        pastAssignments.Any(a => a.Id == s.AssignmentId))
            .ToList();

        var submissionDict = studentSubmissions.ToDictionary(s => s.AssignmentId, s => s);

        // Build performance items
        var performanceItems = pastAssignments.Select(assignment =>
        {
            var hasSubmission = submissionDict.TryGetValue(assignment.Id, out var submission);
            var status = hasSubmission ? "Submitted" : "Missing";
            
            return new AssignmentPerformanceItemResponse
            {
                AssignmentId = assignment.Id,
                Title = assignment.Title,
                DueDate = assignment.DueDate,
                SubmittedAt = hasSubmission ? submission.SubmittedAt : null,
                GradedAt = hasSubmission ? submission.GradedAt : null,
                Grade = hasSubmission ? submission.Grade : null,
                Status = status
            };
        }).ToList();

        // Calculate summary statistics
        var totalAssignments = performanceItems.Count;
        var submittedCount = performanceItems.Count(p => p.Status == "Submitted");
        var missingCount = performanceItems.Count(p => p.Status == "Missing");
        var gradedCount = performanceItems.Count(p => p.GradedAt.HasValue);
        
        // Average grade: calculate from graded submissions only; if none graded, return 0
        var gradedAssignments = performanceItems.Where(p => p.Grade.HasValue).ToList();
        var averageGrade = gradedAssignments.Count > 0 
            ? Math.Round(gradedAssignments.Average(p => (decimal)p.Grade), 2) 
            : 0;

        // Submitted on time: all submissions before or on due date (all are on-time since late not allowed)
        var submittedOnTimeCount = submittedCount;

        var summary = new AssignmentPerformanceSummaryResponse
        {
            TotalAssignments = totalAssignments,
            SubmittedCount = submittedCount,
            MissingCount = missingCount,
            GradedCount = gradedCount,
            AverageGrade = averageGrade,
            SubmittedOnTimeCount = submittedOnTimeCount
        };

        var performanceResponse = new AssignmentPerformanceResponse
        {
            Assignments = performanceItems,
            Summary = summary
        };

        return new SuccessResponse<AssignmentPerformanceResponse> { Data = performanceResponse };
    }
}
