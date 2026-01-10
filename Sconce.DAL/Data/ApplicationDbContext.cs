using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Identity Related
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<InstructorApplication> InstructorApplications { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentApplication> StudentApplications { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<ParentInvite> ParentInvites { get; set; }
        public DbSet<ParentLink> ParentLinks { get; set; }

        // Course Hierarchy
        public DbSet<Program> Programs { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<ProgramEnrollment> ProgramEnrollments { get; set; }

        // Materials
        public DbSet<Content> Contents { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<ZoomMeeting> ZoomMeetings { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public DbSet<EssayQuestion> EssayQuestions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<StudentSection> StudentSections { get; set; }

        // Other
        public DbSet<Dropout> Dropouts { get; set; }
        public DbSet<InformationRequest> InformationRequests { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        { 
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UsersRoles");
            // Ignore tables we don't need
            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityRoleClaim<string>>();

            // Student Parent
            builder.Entity<StudentParent>(entity =>
            {
                entity.HasKey(sp => new { sp.StudentId, sp.ParentId });

                entity.HasOne(sp => sp.Student)
                    .WithMany(s => s.StudentParents)
                    .HasForeignKey(sp => sp.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sp => sp.Parent)
                    .WithMany(p => p.StudentParents)
                    .HasForeignKey(sp => sp.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StudentSection (many-to-many between Student and Section)
            builder.Entity<StudentSection>(entity =>
            {
                entity.HasKey(ss => new { ss.StudentId, ss.SectionId });

                entity.HasOne(ss => ss.Student)
                    .WithMany(s => s.StudentSections)
                    .HasForeignKey(ss => ss.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ss => ss.Section)
                    .WithMany(sec => sec.StudentSections)
                    .HasForeignKey(ss => ss.SectionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Program Enrollment
            builder.Entity<ProgramEnrollment>(entity =>
            {
                entity.HasOne(pe => pe.Program)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(pe => pe.ProgramId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pe => pe.Student)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(pe => pe.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pe => pe.ProficiencyExamAttempt)
                    .WithMany()
                    .HasForeignKey(pe => pe.ProficiencyExamAttemptId)
                    .OnDelete(DeleteBehavior.Restrict); // To avoid SQL Server’s “multiple cascade paths” error

                entity.HasOne(pe => pe.RecommendedCourse)
                    .WithMany()
                    .HasForeignKey(pe => pe.RecommendedCourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pe => pe.EvaluatedByInstructor)
                    .WithMany()
                    .HasForeignKey(pe => pe.EvaluatedByInstructorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pe => pe.PlacedSection)
                    .WithMany()
                    .HasForeignKey(pe => pe.PlacedSectionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint: one enrollment per student per program
                entity.HasIndex(pe => new { pe.ProgramId, pe.StudentId })
                    .IsUnique();
            });

            builder.Entity<Program>(entity =>
            {
                entity.HasOne(p => p.ProficiencyExam)
                    .WithMany()
                    .HasForeignKey(p => p.ProficiencyExamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.ExamWriterInstructor)
                    .WithMany()
                    .HasForeignKey(p => p.ExamWriterInstructorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.EvaluatorInstructor)
                    .WithMany()
                    .HasForeignKey(p => p.EvaluatorInstructorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Program → Levels (Cascade)
            builder.Entity<Level>(entity =>
            {
                entity.HasOne(l => l.Program)
                    .WithMany(p => p.Levels)
                    .HasForeignKey(l => l.ProgramId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.PrerequisiteLevel)
                    .WithMany()
                    .HasForeignKey(l => l.PrerequisiteLevelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Level → Courses (Cascade)
            builder.Entity<Course>(entity =>
            {
                entity.HasOne(c => c.Level)
                    .WithMany(l => l.Courses)
                    .HasForeignKey(c => c.LevelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Course → Sections (Cascade)
            builder.Entity<Section>(entity =>
            {
                entity.HasOne(s => s.Course)
                    .WithMany(c => c.Sections)
                    .HasForeignKey(s => s.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Instructor)
                    .WithMany()
                    .HasForeignKey(s => s.InstructorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MinGrade & MaxGrade precision in Assignment
            builder.Entity<Assignment>(entity =>
            {
                entity.Property(a => a.MinGrade).HasColumnType("decimal(5,2)");
                entity.Property(a => a.MaxGrade).HasColumnType("decimal(5,2)");
            });

            // Grade precision in Submission
            builder.Entity<Submission>(entity =>
            {
                entity.Property(s => s.Grade).HasColumnType("decimal(5,2)");
            });

            // Question inheritance (TPT - Table Per Type)
            builder.Entity<MultipleChoiceQuestion>(entity =>
            {
                entity.Property(mc => mc.AllowMultipleSelections)
                    .HasDefaultValue(false);
                entity.Property(mc => mc.ShuffleChoices)
                    .HasDefaultValue(true);
            });

            builder.Entity<EssayQuestion>(entity =>
            {
                entity.Property(e => e.AllowFileUpload)
                    .HasDefaultValue(false);
            });

            builder.Entity<Choice>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.MultipleChoiceQuestion)
                    .WithMany(q => q.Choices)
                    .HasForeignKey(c => c.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.Text)
                      .IsRequired()
                      .HasMaxLength(450);

                entity.Property(c => c.IsCorrect)
                    .HasDefaultValue(false);
            });

            // Content (base) → Section (Cascade to delete all derived contents like Exams, Assignments, Texts, ZoomMeetings)
            builder.Entity<Content>(entity =>
            {
                entity.HasOne(c => c.Section)
                    .WithMany()
                    .HasForeignKey(c => c.SectionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Document>(entity =>
            {
                entity.Property(d => d.Title)
                    .HasMaxLength(200);
            });

            builder.Entity<Exam>(entity =>
            {
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.AttemptsAllowed)
                    .HasDefaultValue(1);

                entity.Property(e => e.ShuffleQuestions)
                    .HasDefaultValue(false);
            });

            builder.Entity<ExamQuestion>(entity =>
            {
                entity.Property(eq => eq.Points)
                    .HasColumnType("decimal(6,2)");

                entity.HasOne(eq => eq.Exam)
                    .WithMany()
                    .HasForeignKey(eq => eq.ExamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(eq => eq.Question)
                    .WithMany()
                    .HasForeignKey(eq => eq.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint: (ExamId, SortOrder)
                entity.HasIndex(eq => new { eq.ExamId, eq.SortOrder })
                    .IsUnique();

                // Unique constraint: (ExamId, QuestionId) to prevent duplicates
                entity.HasIndex(eq => new { eq.ExamId, eq.QuestionId })
                    .IsUnique();
            });

            builder.Entity<ExamAttempt>(entity =>
            {
                entity.HasOne(ea => ea.Exam)
                    .WithMany()
                    .HasForeignKey(ea => ea.ExamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ea => ea.Student)
                    .WithMany()
                    .HasForeignKey(ea => ea.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(ea => ea.AttemptNumber)
                    .HasDefaultValue(1);

                entity.Property(ea => ea.Score)
                    .HasColumnType("decimal(6,2)");

                entity.Property(ea => ea.MaxScore)
                    .HasColumnType("decimal(6,2)");

                builder.Entity<ExamAttempt>()
                    .HasIndex(e => new { e.ExamId, e.StudentId })
                    .HasFilter("[AttemptStatus] = 1")
                    .IsUnique();

                // Unique constraint: one attempt number per student per exam
                entity.HasIndex(ea => new { ea.ExamId, ea.StudentId, ea.AttemptNumber })
                    .IsUnique();
            });

            builder.Entity<Answer>(entity =>
            {
                entity.HasOne(a => a.ExamAttempt)
                    .WithMany()
                    .HasForeignKey(a => a.ExamAttemptId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.ExamQuestion)
                    .WithMany()
                    .HasForeignKey(a => a.ExamQuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.MaxScore)
                    .HasColumnType("decimal(6,2)");
                entity.Property(a => a.Score)
                    .HasColumnType("decimal(6,2)");

                // Unique constraint: one answer per (ExamAttemptId, ExamQuestionId)
                entity.HasIndex(a => new { a.ExamAttemptId, a.ExamQuestionId })
                    .IsUnique();
            });

            // Course → Questions (Cascade)
            builder.Entity<Question>(entity =>
            {
                entity.HasOne(q => q.Course)
                    .WithMany(c => c.Questions)
                    .HasForeignKey(q => q.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(q => q.Program)
                    .WithMany()
                    .HasForeignKey(q => q.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
