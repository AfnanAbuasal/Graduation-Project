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

namespace Sconce.BLL.Services.Classes
{
    public class CourseService : GenericService<CourseRequest, CourseResponse, Course>, ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IProgramRepository _programRepository;
        public CourseService(ICourseRepository courseRepository, IProgramRepository programRepository) : base(courseRepository)
        {
            _courseRepository = courseRepository;
            _programRepository = programRepository;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(CourseRequest request)
        {
            // Ensure Program exists before creating course
            var program = await _programRepository.GetByIdAsync(request.ProgramId);
            if (program == null) return (0, new ErrorResponse { Errors = [$"Program with Id: {request.ProgramId} not found."] });

            // Validate course dates are logical
            if (request.StartDate > request.EndDate)
                return (0, new ErrorResponse { Errors = ["Course StartDate must be on or before EndDate."] });

            // Validate course duration fits within program duration
            if (request.StartDate < program.StartDate || request.EndDate > program.EndDate)
                return (0, new ErrorResponse { Errors = [
                    $"Course dates must be within program window {program.StartDate:yyyy-MM-dd} to {program.EndDate:yyyy-MM-dd}."
                ] });

            // Validate no overlap with other courses in the same program
            var allCourses = await _courseRepository.GetAllAsync();
            var overlapping = allCourses
                .Where(c => c.ProgramId == request.ProgramId)
                .Any(c => request.StartDate <= c.EndDate && request.EndDate >= c.StartDate);

            if (overlapping)
                return (0, new ErrorResponse { Errors = ["Course dates overlap with another course in this program."] });

            var course = request.Adapt<Course>();
            var rows = await _courseRepository.AddAsync(course);

            // Increment ActualCourseCount
            if (rows > 0)
            {
                program.ActualCourseCount++;
                await _programRepository.UpdateAsync(program);
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int ID, CourseRequest request)
        {
            // Ensure existing course
            var existing = await _courseRepository.GetByIdAsync(ID);
            if (existing == null)
                return (0, new ErrorResponse { Errors = ["Not Found."] });

            // Ensure target program exists
            var program = await _programRepository.GetByIdAsync(request.ProgramId);
            if (program == null)
                return (0, new ErrorResponse { Errors = [$"Program with Id: {request.ProgramId} not found."] });

            // Validate logical dates
            if (request.StartDate > request.EndDate)
                return (0, new ErrorResponse { Errors = ["Course StartDate must be on or before EndDate."] });

            // Validate within program boundaries
            if (request.StartDate < program.StartDate || request.EndDate > program.EndDate)
                return (0, new ErrorResponse { Errors = [
                    $"Course dates must be within program window {program.StartDate:yyyy-MM-dd} to {program.EndDate:yyyy-MM-dd}."
                ] });

            // Validate no overlap with other courses in the same program (exclude self)
            var allCourses = await _courseRepository.GetAllAsync();
            var overlapping = allCourses
                .Where(c => c.ProgramId == request.ProgramId && c.Id != ID)
                .Any(c => request.StartDate <= c.EndDate && request.EndDate >= c.StartDate);

            if (overlapping)
                return (0, new ErrorResponse { Errors = ["Course dates overlap with another course in this program."] });

            // Apply updates
            request.Adapt(existing);
            var rows = await _courseRepository.UpdateAsync(existing);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }
    }
}
