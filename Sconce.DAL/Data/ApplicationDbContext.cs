using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Models;
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

        // Materials

        public DbSet<Content> Contents { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<ZoomMeeting> ZoomMeetings { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Dropout> Dropouts { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public DbSet<EssayQuestion> EssayQuestions { get; set; }
        public DbSet<Choice> Choices { get; set; }

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
                entity.HasKey(c => new { c.QuestionId, c.Text });

                entity.HasOne(c => c.Question)
                    .WithMany()
                    .HasForeignKey(c => c.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.IsCorrect)
                    .HasDefaultValue(false);

                entity.HasIndex(c => c.SortOrder)
                    .IsUnique();
            });
        }
    }
}
