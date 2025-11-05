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

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
            builder.Services.AddScoped<IFileUrlHelper, FileUrlHelper>();
            builder.Services.AddScoped<IStudentApplicationRepository, StudentApplicationRepository>();
            builder.Services.AddScoped<IStudentApplicationService, StudentApplicationService>();
            builder.Services.AddScoped<IAdminStudentService, AdminStudentService>();
            builder.Services.AddScoped<IParentInviteRepository, ParentInviteRepository>();
            builder.Services.AddScoped<IParentLinkRepository, ParentLinkRepository>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                options => {
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

            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<ISeedData>();
                //await objectOfSeedData.DataSeedingAsync();
                await seeder.IdentityDataSeedingAsync();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
