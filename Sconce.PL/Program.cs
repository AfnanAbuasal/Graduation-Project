using Sconce.DAL.Utilities;
using Sconce.PL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using System.Text;
using Sconce.BLL.Services.Interfaces;
using Sconce.BLL.Services.Classes;
using Sconce.DAL.Repositories.Interfaces;
using Sconce.DAL.Repositories.Classes;

namespace Sconce.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Cors Policy
            var userPolicy = "FrontendPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: userPolicy, policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            // Database Connection
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Dependency Injections
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericService<,,>), typeof(GenericService<,,>));
            builder.Services.AddScoped<IInstructorApplicationRepository, InstructorApplicationRepository>();
            builder.Services.AddScoped<IInstructorApplicationService, InstructorApplicationService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IEmailSender, EmailSetting>();
            builder.Services.AddScoped<ISeedData, SeedData>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IAdminInstructorService, AdminInstructorService>();
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IUrlHelper, UrlHelper>();
            builder.Services.AddScoped<IStudentApplicationRepository, StudentApplicationRepository>();
            builder.Services.AddScoped<IStudentApplicationService, StudentApplicationService>();
            builder.Services.AddScoped<IAdminStudentService, AdminStudentService>();
            builder.Services.AddScoped<IParentInviteRepository, ParentInviteRepository>();
            builder.Services.AddScoped<IParentLinkRepository, ParentLinkRepository>();
            builder.Services.AddScoped<IStudentParentRepository, StudentParentRepository>();
            builder.Services.AddScoped<IProgramService, ProgramService>();
            builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ISectionRepository, SectionRepository>();
            builder.Services.AddScoped<ISectionService, SectionService>();
            builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            builder.Services.AddScoped<IAssignmentService, AssignmentService>();
            builder.Services.AddScoped<ITextRepository, TextRepository>();
            builder.Services.AddScoped<ITextService, TextService>();
            builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            builder.Services.AddScoped<ISubmissionService, SubmissionService>();
            builder.Services.AddScoped<IDropoutRepository, DropoutRepository>();
            builder.Services.AddScoped<IDropoutService, DropoutService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IDbService, DbService>();
            builder.Services.AddScoped<IContentRepository, ContentRepository>();
            builder.Services.AddScoped<IContentService, ContentService>();
            builder.Services.AddScoped<IZoomMeetingRepository, ZoomMeetingRepository>();
            builder.Services.AddScoped<IZoomMeetingService, ZoomMeetingService>();

            // Identity Configuration
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.User.RequireUniqueEmail = true;
                }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // JWT Authentication Configuration.
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Use JWT Bearer instead of cookies.
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // 401 instead of 404 with unauthorized requests.
            })
            .AddJwtBearer(options =>
            {
                // What to check to validate the token:
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    //ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    //ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("jwtOptions")["SecretKey"])) // Secret key.
                };
            });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapScalarApiReference(options =>
                {
                    options.WithOpenApiRoutePattern("swagger/v1/swagger.json");
                });
            }

            // Data Seeding
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<ISeedData>();
                //await objectOfSeedData.DataSeedingAsync();
                await seeder.IdentityDataSeedingAsync();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors(userPolicy);

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
