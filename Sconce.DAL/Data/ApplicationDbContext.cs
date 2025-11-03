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

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UsersRoles");
            //Ignore other tables
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

        }
    }
}
