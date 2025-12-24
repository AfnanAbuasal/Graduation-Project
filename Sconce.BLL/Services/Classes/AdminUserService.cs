using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.Data;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUrlHelper _urlHelper;
        private readonly ApplicationDbContext _context;

        public AdminUserService(UserManager<ApplicationUser> userManager, IUrlHelper urlHelper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _urlHelper = urlHelper;
            _context = context;
        }

        public async Task<Response> GetAllUserProfilesAsync(UserType? userType = null)
        {
            var profiles = new List<UserProfileResponse>();

            if (!userType.HasValue || userType == UserType.Student)
            {
                var students = await _userManager.Users
                    .OfType<Student>()
                    .AsNoTracking()
                    .ToListAsync();

                profiles.AddRange(students.Select(MapStudent));
            }

            if (!userType.HasValue || userType == UserType.Instructor)
            {
                var instructors = await _userManager.Users
                    .OfType<Instructor>()
                    .AsNoTracking()
                    .ToListAsync();

                profiles.AddRange(instructors.Select(MapInstructor));
            }

            if (!userType.HasValue || userType == UserType.Parent)
            {
                var parents = await _userManager.Users
                    .OfType<Parent>()
                    .AsNoTracking()
                    .ToListAsync();

                profiles.AddRange(parents.Select(MapParent));
            }

            return new SuccessResponse<IEnumerable<UserProfileResponse>> { Data = profiles };
        }

        public async Task<(bool Success, Response Response)> GetUserProfileByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return (false, new ErrorResponse { Errors = new List<string> { "User not found." } });

            UserProfileResponse? profile = user switch
            {
                Student student => MapStudent(student),
                Instructor instructor => MapInstructor(instructor),
                Parent parent => MapParent(parent),
                _ => null
            };

            if (profile == null)
                return (false, new ErrorResponse { Errors = new List<string> { "Invalid user type." } });

            return (true, new SuccessResponse<UserProfileResponse> { Data = profile });
        }

        private StudentProfileResponse MapStudent(Student student)
        {
            return new StudentProfileResponse
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Country = student.Country,
                City = student.City,
                Street = student.Street,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                UserType = UserType.Student,
                LevelOfProficiency = student.LevelOfProficiency,
                DocumentPath = student.DocumentPath,
                DocumentUrl = _urlHelper.BuildUrl(student.DocumentPath)
            };
        }

        private InstructorProfileResponse MapInstructor(Instructor instructor)
        {
            return new InstructorProfileResponse
            {
                Id = instructor.Id,
                FullName = instructor.FullName,
                Email = instructor.Email,
                PhoneNumber = instructor.PhoneNumber,
                Country = instructor.Country,
                City = instructor.City,
                Street = instructor.Street,
                DateOfBirth = instructor.DateOfBirth,
                Gender = instructor.Gender,
                UserType = UserType.Instructor,
                YearsOfExperience = instructor.YearsOfExperience,
                ExperienceWithTeachingKids = instructor.ExperienceWithTeachingKids,
                CVPath = instructor.CVPath,
                CVUrl = _urlHelper.BuildUrl(instructor.CVPath)
            };
        }

        private ParentProfileResponse MapParent(Parent parent)
        {
            return new ParentProfileResponse
            {
                Id = parent.Id,
                FullName = parent.FullName,
                Email = parent.Email,
                PhoneNumber = parent.PhoneNumber,
                Country = parent.Country,
                City = parent.City,
                Street = parent.Street,
                DateOfBirth = parent.DateOfBirth,
                Gender = parent.Gender,
                UserType = UserType.Parent
            };
        }

        public async Task<(bool Success, Response Response)> DeleteStudentByIdAsync(string studentId, bool deleteApplication = false)
        {
            var student = await _userManager.Users.OfType<Student>()
                .Include(s => s.StudentParents)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return (false, new ErrorResponse { Errors = new List<string> { "Student not found." } });

            // 1. Always delete StudentParent links (join table)
            if (student.StudentParents.Any())
            {
                _context.StudentParents.RemoveRange(student.StudentParents);
            }

            // 2. Optionally delete application
            if (deleteApplication)
            {
                var application = await _context.StudentApplications
                    .FirstOrDefaultAsync(a => a.Email == student.Email);
                if (application != null)
                {
                    _context.StudentApplications.Remove(application);
                }
            }

            // 3. Delete student
            var result = await _userManager.DeleteAsync(student);
            if (!result.Succeeded)
                return (false, new ErrorResponse { Errors = result.Errors.Select(e => e.Description).ToList() });

            await _context.SaveChangesAsync();
            return (true, new SuccessResponse<string> { Data = "Student deleted successfully." });
        }

        public async Task<(bool Success, Response Response)> DeleteInstructorByIdAsync(string instructorId, bool deleteApplication = false)
        {
            var instructor = await _userManager.Users.OfType<Instructor>()
                .FirstOrDefaultAsync(i => i.Id == instructorId);

            if (instructor == null)
                return (false, new ErrorResponse { Errors = new List<string> { "Instructor not found." } });

            // 1. Optionally delete application
            if (deleteApplication)
            {
                var application = await _context.InstructorApplications
                    .FirstOrDefaultAsync(a => a.Email == instructor.Email);
                if (application != null)
                {
                    _context.InstructorApplications.Remove(application);
                }
            }

            // 2. Delete instructor
            var result = await _userManager.DeleteAsync(instructor);
            if (!result.Succeeded)
                return (false, new ErrorResponse { Errors = result.Errors.Select(e => e.Description).ToList() });

            await _context.SaveChangesAsync();
            return (true, new SuccessResponse<string> { Data = "Instructor deleted successfully." });
        }
        public async Task<(bool Success, Response Response)> DeleteParentByIdAsync(string parentId)
        {
            var parent = await _userManager.Users.OfType<Parent>()
                .Include(p => p.StudentParents) // Load links
                .FirstOrDefaultAsync(p => p.Id == parentId);

            if (parent == null)
                return (false, new ErrorResponse { Errors = new List<string> { "Parent not found." } });

            // Auto-delete student links
            if (parent.StudentParents.Any())
            {
                _context.StudentParents.RemoveRange(parent.StudentParents);
            }

            // Delete parent
            var result = await _userManager.DeleteAsync(parent);
            if (!result.Succeeded)
                return (false, new ErrorResponse { Errors = result.Errors.Select(e => e.Description).ToList() });

            await _context.SaveChangesAsync();
            return (true, new SuccessResponse<string> { Data = "Parent deleted successfully." });
        }
    }
}
