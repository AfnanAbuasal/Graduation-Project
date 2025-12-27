using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Utilities
{
    public class SeedData : ISeedData
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public SeedData(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task DataSeedingAsync()
        {
            // Check if data already exists
            if (await _context.Programs.AnyAsync())
                return;

            // Get the instructor user
            var instructorUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == "johninstructor");
            var instructorId = instructorUser?.Id;

            // Create Arabic Program
            var arabicProgram = new Program
            {
                Name = "Arabic Program",
                Description = "A comprehensive Arabic language learning program",
                PlannedLevelCount = 3,
                ActualLevelCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            _context.Programs.Add(arabicProgram);
            await _context.SaveChangesAsync();

            // Create Levels
            var beginnerLevel = new Level
            {
                Name = "Beginner",
                Description = "For learners starting Arabic from scratch",
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3)),
                PlannedCourseCount = 2,
                ActualCourseCount = 0,
                ProgramId = arabicProgram.Id,
                CreatedAt = DateTime.UtcNow
            };

            var intermediateLevel = new Level
            {
                Name = "Intermediate",
                Description = "For learners with basic Arabic knowledge",
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                PlannedCourseCount = 2,
                ActualCourseCount = 0,
                ProgramId = arabicProgram.Id,
                PrerequisiteLevelId = null,
                CreatedAt = DateTime.UtcNow
            };

            var advancedLevel = new Level
            {
                Name = "Advanced",
                Description = "For advanced Arabic learners",
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(9)),
                PlannedCourseCount = 2,
                ActualCourseCount = 0,
                ProgramId = arabicProgram.Id,
                PrerequisiteLevelId = null,
                CreatedAt = DateTime.UtcNow
            };

            _context.Levels.AddRange(beginnerLevel, intermediateLevel, advancedLevel);
            await _context.SaveChangesAsync();

            // Create Courses
            var a1Course = new Course
            {
                Name = "A1",
                Description = "Arabic Level A1 Course",
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1).AddDays(15)),
                Order = 1,
                LevelId = beginnerLevel.Id,
                CreatedAt = DateTime.UtcNow
            };

            var a2Course = new Course
            {
                Name = "A2",
                Description = "Arabic Level A2 Course",
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1).AddDays(15)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3)),
                Order = 2,
                LevelId = beginnerLevel.Id,
                CreatedAt = DateTime.UtcNow
            };

            var b1Course = new Course
            {
                Name = "B1",
                Description = "Arabic Level B1 Course",
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(4).AddDays(15)),
                Order = 1,
                LevelId = intermediateLevel.Id,
                CreatedAt = DateTime.UtcNow
            };

            var b2Course = new Course
            {
                Name = "B2",
                Description = "Arabic Level B2 Course",
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(4).AddDays(15)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                Order = 2,
                LevelId = intermediateLevel.Id,
                CreatedAt = DateTime.UtcNow
            };

            var c1Course = new Course
            {
                Name = "C1",
                Description = "Arabic Level C1 Course",
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(7).AddDays(15)),
                Order = 1,
                LevelId = advancedLevel.Id,
                CreatedAt = DateTime.UtcNow
            };

            var c2Course = new Course
            {
                Name = "C2",
                Description = "Arabic Level C2 Course",
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(7).AddDays(15)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(9)),
                Order = 2,
                LevelId = advancedLevel.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Courses.AddRange(a1Course, a2Course, b1Course, b2Course, c1Course, c2Course);
            await _context.SaveChangesAsync();

            // Create Section in A1 Course
            var section = new Section
            {
                Name = "Section A1-001",
                Capacity = 30,
                CourseId = a1Course.Id,
                InstructorId = instructorId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            // Create Exam in the Section
            var exam = new Exam
            {
                Title = "A1 - Alphabet and Basics Quiz",
                SectionId = section.Id,
                WeekNumber = 1,
                AvailableFrom = DateTime.UtcNow,
                AvailableTo = DateTime.UtcNow.AddDays(7),
                DurationMinutes = 30,
                AttemptsAllowed = 3,
                ShuffleQuestions = true,
                ExamStatus = ExamStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            _context.Add(exam);
            await _context.SaveChangesAsync();

            // Create MultipleChoiceQuestion in the Exam
            var mcQuestion = new MultipleChoiceQuestion
            {
                Prompt = "What is the first letter of the Arabic alphabet?",
                Difficulty = Difficulty.Easy,
                CreatedByInstructorId = instructorId,
                CourseId = a1Course.Id,
                AllowMultipleSelections = false,
                ShuffleChoices = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Questions.Add(mcQuestion);
            await _context.SaveChangesAsync();

            // Create Choices for the MCQuestion
            var choices = new List<Choice>
            {
                new Choice
                {
                    QuestionId = mcQuestion.Id,
                    Text = "Alef (ا)",
                    IsCorrect = true
                },
                new Choice
                {
                    QuestionId = mcQuestion.Id,
                    Text = "Baa (ب)",
                    IsCorrect = false
                },
                new Choice
                {
                    QuestionId = mcQuestion.Id,
                    Text = "Taa (ت)",
                    IsCorrect = false
                }
            };

            _context.Choices.AddRange(choices);
            await _context.SaveChangesAsync();
        }

        public async Task IdentityDataSeedingAsync()
        {
            var defaultPassword = _configuration["SeedSettings:DefaultPassword"] ?? "Temp@123"; // fallback just in case

            foreach (var roleName in new[] { "Super Admin", "Admin", "Student", "Instructor", "Parent" })
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            if (!await _userManager.Users.AnyAsync())
            {
                var user1 = new ApplicationUser()
                {
                    Email = "anas.melhem@gmail.com",
                    FullName = "Anas Melhem",
                    UserName = "anasmelhem",
                    PhoneNumber = "0598765432",
                    Country = "Palestine",
                    City = "Tulkarm",
                    Street = "Yafa Street",
                    EmailConfirmed = true
                };
                var user2 = new ApplicationUser()
                {
                    Email = "layla.saabna@gmail.com",
                    FullName = "Layla Sa'abna",
                    UserName = "laylasaabna",
                    PhoneNumber = "0598765432",
                    Country = "Palestine",
                    City = "Jenin",
                    Street = "Fahma Main Street",
                    EmailConfirmed = true
                };
                var instructorUser = new ApplicationUser()
                {
                    Email = "instructor@example.com",
                    FullName = "John Instructor",
                    UserName = "johninstructor",
                    PhoneNumber = "0591234567",
                    Country = "Palestine",
                    City = "Ramallah",
                    Street = "Main Street",
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(user1, defaultPassword);
                await _userManager.CreateAsync(user2, defaultPassword);
                await _userManager.CreateAsync(instructorUser, defaultPassword);

                await _userManager.AddToRoleAsync(user1, "Admin");
                await _userManager.AddToRoleAsync(user2, "Super Admin");
                await _userManager.AddToRoleAsync(instructorUser, "Instructor");
            }
        }
    }
}
