using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
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
        private readonly ILevelRepository _levelRepository;
        public CourseService(ICourseRepository courseRepository, ILevelRepository levelRepository) : base(courseRepository)
        {
            _courseRepository = courseRepository;
            _levelRepository = levelRepository;
        }

        public override async Task<Response> GetAllAsync(bool onlyActive = false)
        {
            var list = await _courseRepository.GetAllWithLevelAsync();

            if (onlyActive)
                list = list.Where(x => x.Status == Status.Active);

            var responseList = new List<CourseResponse>();
            foreach (var entity in list)
            {
                var dto = entity.Adapt<CourseResponse>();
                dto.LevelName = entity.Level?.Name;
                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<CourseResponse>> { Data = responseList };
        }

        public override async Task<(bool Success, Response Response)> GetByIdAsync(int Id)
        {
            var entity = await _courseRepository.GetByIdWithLevelAsync(Id);
            if (entity == null)
                return (false, new ErrorResponse { Errors = ["Not Found."] });

            var dto = entity.Adapt<CourseResponse>();
            dto.LevelName = entity.Level?.Name;
            return (true, new SuccessResponse<CourseResponse> { Data = dto });
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(CourseRequest request)
        {
            // Ensure Level exists before creating course
            var level = await _levelRepository.GetByIdAsync(request.LevelId);
            if (level == null) return (0, new ErrorResponse { Errors = [$"Level with Id: {request.LevelId} not found."] });

            // Validate that level has not reached its planned course count
            if (level.ActualCourseCount >= level.PlannedCourseCount)
                return (0, new ErrorResponse { Errors = [
                    $"Cannot create course. Level has reached its planned course count ({level.PlannedCourseCount})."
                ] });

            // Validate course dates are logical
            if (request.StartDate > request.EndDate)
                return (0, new ErrorResponse { Errors = ["Course StartDate must be on or before EndDate."] });

            // Validate course duration fits within level duration
            if (request.StartDate < level.StartDate || request.EndDate > level.EndDate)
                return (0, new ErrorResponse { Errors = [
                    $"Course dates must be within level window {level.StartDate:yyyy-MM-dd} to {level.EndDate:yyyy-MM-dd}."
                ] });

            // Validate order is within valid range
            if (request.Order < 1 || request.Order > level.PlannedCourseCount)
                return (0, new ErrorResponse { Errors = [
                    $"Course order must be between 1 and {level.PlannedCourseCount} (Planned Course Count)."
                ] });

            // Validate no overlap with other courses in the same level
            var allCourses = await _courseRepository.GetAllAsync();
            var overlapping = allCourses
                .Where(c => c.LevelId == request.LevelId)
                .Any(c => request.StartDate <= c.EndDate && request.EndDate >= c.StartDate);

            if (overlapping)
                return (0, new ErrorResponse { Errors = ["Course dates overlap with another course in this level."] });

            // Validate order is unique within the level
            var orderExists = allCourses
                .Where(c => c.LevelId == request.LevelId)
                .Any(c => c.Order == request.Order);

            if (orderExists)
                return (0, new ErrorResponse { Errors = [$"A course with order {request.Order} already exists in this level."] });

            var course = request.Adapt<Course>();
            var rows = await _courseRepository.AddAsync(course);

            // Increment ActualCourseCount
            if (rows > 0)
            {
                level.ActualCourseCount++;
                level.UpdatedAt = DateTime.UtcNow;
                await _levelRepository.UpdateAsync(level);
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int ID, CourseRequest request)
        {
            // Ensure existing course
            var existing = await _courseRepository.GetByIdAsync(ID);
            if (existing == null)
                return (0, new ErrorResponse { Errors = ["Not Found."] });

            // Ensure target level exists
            var level = await _levelRepository.GetByIdAsync(request.LevelId);
            if (level == null)
                return (0, new ErrorResponse { Errors = [$"Level with Id: {request.LevelId} not found."] });

            // Validate logical dates
            if (request.StartDate > request.EndDate)
                return (0, new ErrorResponse { Errors = ["Course StartDate must be on or before EndDate."] });

            // Validate within level boundaries
            if (request.StartDate < level.StartDate || request.EndDate > level.EndDate)
                return (0, new ErrorResponse { Errors = [
                    $"Course dates must be within level window {level.StartDate:yyyy-MM-dd} to {level.EndDate:yyyy-MM-dd}."
                ] });

            // Validate order is within valid range
            if (request.Order < 1 || request.Order > level.PlannedCourseCount)
                return (0, new ErrorResponse { Errors = [
                    $"Course order must be between 1 and {level.PlannedCourseCount} (Planned Course Count)."
                ] });

            // Validate no overlap with other courses in the same level (exclude self)
            var allCourses = await _courseRepository.GetAllAsync();
            var overlapping = allCourses
                .Where(c => c.LevelId == request.LevelId && c.Id != ID)
                .Any(c => request.StartDate <= c.EndDate && request.EndDate >= c.StartDate);

            if (overlapping)
                return (0, new ErrorResponse { Errors = ["Course dates overlap with another course in this level."] });

            // Validate order is unique within the level (exclude self)
            var orderExists = allCourses
                .Where(c => c.LevelId == request.LevelId && c.Id != ID)
                .Any(c => c.Order == request.Order);

            if (orderExists)
                return (0, new ErrorResponse { Errors = [$"A course with order {request.Order} already exists in this level."] });

            // If level changed, update both old and new level counts
            var oldLevelId = existing.LevelId;
            var newLevelId = request.LevelId;

            // Apply updates
            request.Adapt(existing);
            existing.UpdatedAt = DateTime.UtcNow;
            var rows = await _courseRepository.UpdateAsync(existing);

            if (rows > 0 && oldLevelId != newLevelId)
            {
                // Decrement old level count
                var oldLevel = await _levelRepository.GetByIdAsync(oldLevelId);
                if (oldLevel != null)
                {
                    oldLevel.ActualCourseCount--;
                    oldLevel.UpdatedAt = DateTime.UtcNow;
                    await _levelRepository.UpdateAsync(oldLevel);
                }

                // Increment new level count
                level.ActualCourseCount++;
                level.UpdatedAt = DateTime.UtcNow;
                await _levelRepository.UpdateAsync(level);
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int ID)
        {
            // Get the course to be deleted
            var course = await _courseRepository.GetByIdAsync(ID);
            if (course == null)
                return (0, new ErrorResponse { Errors = ["Not Found."] });

            var levelId = course.LevelId;

            // Delete the course
            var rows = await _courseRepository.DeleteAsync(course);

            // Decrement ActualCourseCount if deletion was successful
            if (rows > 0)
            {
                var level = await _levelRepository.GetByIdAsync(levelId);
                if (level != null)
                {
                    level.ActualCourseCount--;
                    level.UpdatedAt = DateTime.UtcNow;
                    await _levelRepository.UpdateAsync(level);
                }
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) deleted successfully." });
        }
        public async Task<Response> GetByLevelAsync(int levelId, bool onlyActive = false)
        {
            var courses = await _courseRepository.GetByLevelIdAsync(levelId, onlyActive);

            var responseList = new List<CourseResponse>();
            foreach (var entity in courses)
            {
                var dto = entity.Adapt<CourseResponse>();
                dto.LevelName = entity.Level?.Name;
                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<CourseResponse>> { Data = responseList };
        }

        public async Task<Response> GetByProgramAsync(int programId, bool onlyActive = false)
        {
            var levels = await _levelRepository.GetAllByProgramWithCoursesAsync(programId);
            var levelList = levels.ToList();

            if (!levelList.Any())
                return new SuccessResponse<IEnumerable<CourseResponse>> { Data = [] };

            var responseList = new List<CourseResponse>();
            foreach (var level in levelList)
            {
                var courses = onlyActive
                    ? level.Courses.Where(c => c.Status == Status.Active)
                    : level.Courses;

                foreach (var course in courses.OrderBy(c => c.Order).ThenBy(c => c.Id))
                {
                    var dto = course.Adapt<CourseResponse>();
                    dto.LevelName = level.Name;
                    responseList.Add(dto);
                }
            }

            return new SuccessResponse<IEnumerable<CourseResponse>> { Data = responseList };
        }
    }
}
