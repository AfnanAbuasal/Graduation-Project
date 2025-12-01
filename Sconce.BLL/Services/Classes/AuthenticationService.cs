using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Sconce.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Models.Enums;

namespace Sconce.BLL.Services.Classes
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IUrlHelper _urlHelper;
        private readonly IParentInviteRepository _parentInviteRepository;
        private readonly IParentLinkRepository _parentLinkRepository;
        private readonly IStudentParentRepository _studentParentRepository;
        private readonly IStudentApplicationRepository _studentApplicationRepository;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            INotificationService notificationService,
            IUrlHelper urlHelper,
            IParentInviteRepository parentInviteRepository,
            IParentLinkRepository parentLinkRepository,
            IStudentParentRepository studentParentRepository,
            IStudentApplicationRepository studentApplicationRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _notificationService = notificationService;
            _urlHelper = urlHelper;
            _parentInviteRepository = parentInviteRepository;
            _parentLinkRepository = parentLinkRepository;
            _studentParentRepository = studentParentRepository;
            _studentApplicationRepository = studentApplicationRepository;
        }

        // General
        public async Task<(bool Success, Response Response)> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user is null)
                return (false, new Response { Message = "Invalid Email or Password." });

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return (false, new Response { Message = "Please Confirm Your Email First." });

            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return (false, new Response { Message = "Invalid Email or Password." });

            return (true, new SuccessResponse { Token = await GenerateTokenAsync(user), Message = "Login Successsfull." });
        }

        private async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.UserData, user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("jwtOptions")["SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                //issuer: _config["Jwt:Issuer"],
                //audience: _config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<(bool Success, Response Response)> ConfirmEmailAsync(string token, string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);

            if (user is null)
                return (false, new Response { Message = "User not Found." });

            if (user.EmailConfirmed)
                return (true, new Response { Message = "Email Already Confirmed." });

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return (false, new ErrorResponse
                {
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                    Message = "Email Confirmation Failed."
                });
            }

            return (true, new Response { Message = "Email Confirmed Successfully!" });
        }

        public async Task<(bool Success, Response Response)> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordRequest.Email);

            if (user is null)
                return (false, new Response { Message = "User not Found." });

            var random = new Random();
            var code = random.Next(1000, 9999).ToString();

            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiration = DateTime.UtcNow.AddMinutes(15);

            await _userManager.UpdateAsync(user);

            await _notificationService.SendPasswordResetCodeAsync(forgotPasswordRequest, code);

            return (true, new Response { Message = "Please check your email inbox." });
        }

        public async Task<(bool Success, Response Response)> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);

            if (user is null)
                return (false, new Response { Message = "User not Found." });

            if (user.PasswordResetCode != resetPasswordRequest.Code)
                return (false, new Response { Message = "Wrong Code." });

            if (user.PasswordResetCodeExpiration < DateTime.UtcNow)
                return (false, new Response { Message = "Code Expired." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordRequest.NewPassword);

            if (result.Succeeded) await _notificationService.SendPasswordResetSuccessAsync(resetPasswordRequest, user);

            return (true, new Response { Message = "Password Reset Successfully!" });
        }

        // Student Registration
        public async Task<(bool Success, Response Response)> RegisterStudentAsync(StudentRegisterRequest registerRequest)
        {
            var student = new Student
            {
                Email = registerRequest.Email,
                FullName = registerRequest.FullName,
                UserName = registerRequest.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(student, registerRequest.Password);
            if (!result.Succeeded)
            {
                return (false, new ErrorResponse
                {
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                    Message = "Registration failed."
                });
            }

            await _userManager.AddToRoleAsync(student, "Student");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(student);
            var escapedToken = Uri.EscapeDataString(token);
            var relativePath = $"/api/Identity/Account/ConfirmEmail?token={escapedToken}&userId={student.Id}";
            var confirmationUrl = _urlHelper.BuildUrl(relativePath);

            await _notificationService.SendConfirmEmailAsync(student, confirmationUrl);

            return (true, new SuccessResponse
            {
                Token = student.Email,
                Message = "Registration successful. Please check your inbox to verify your email."
            });
        }

        public async Task<(bool Success, Response Response)> ApproveParentLinkAsync(string token)
        {
            // Validate the link
            var link = (await _parentLinkRepository.GetAllAsync())
                .FirstOrDefault(l => l.Token == token && !l.IsUsed && l.ExpiresAt > DateTime.UtcNow);

            if (link == null)
                return (false, new Response { Message = "Invalid or expired token." });

            // Mark as approved
            link.IsUsed = true;
            link.IsApproved = true;
            await _parentLinkRepository.UpdateAsync(link);

            // Fetch related users
            var parent = await _userManager.Users
                .OfType<Parent>()
                .FirstOrDefaultAsync(p => p.Id == link.ParentId);
            var student = await _userManager.Users
                .OfType<Student>()
                .FirstOrDefaultAsync(s => s.Email == link.StudentEmail);

            if (parent == null || student == null)
                return (false, new Response { Message = "Parent or student not found." });

            // Create relationship
            var relation = new StudentParent
            {
                StudentId = student.Id,
                ParentId = parent.Id,
                RelationshipWithStudent = link.RelationshipWithStudent,
                LinkedAt = DateTime.UtcNow,
                IsConfirmed = true
            };

            await _studentParentRepository.AddAsync(relation);

            // Create Token to confirm email
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(parent);
            var escapedToken = Uri.EscapeDataString(emailToken);

            var confirmationRelativePath = $"/api/Identity/Account/ConfirmEmail?token={escapedToken}&userID={parent.Id}";
            var emailConfirmationURL = _urlHelper.BuildUrl(confirmationRelativePath);

            // Send notification emails
            await _notificationService.SendParentLinkedAsync(student, parent, link.RelationshipWithStudent);
            await _notificationService.SendStudentLinkedAsync(parent, student, emailConfirmationURL);

            return (true, new Response { Message = "Parent link approved successfully!" });
        }

        // Parent
        public async Task<(bool Success, Response Response)> RegisterParentAsync(ParentRegisterRequest request)
        {
            var student = await _userManager.Users
            .OfType<Student>()
            .FirstOrDefaultAsync(s => s.Email == request.StudentEmail);

            if (student == null)
                return (false, new Response { Message = "No student found with the provided email." });

            var studentApplication = (await _studentApplicationRepository.GetAllAsync())
            .FirstOrDefault(a => a.Email == request.StudentEmail);

            if (studentApplication == null)
                return (false, new Response
                {
                    Message = "The student has not yet submitted their application. Please ask them to submit it first."
                });

            switch (studentApplication.ApplicationStatus)
            {
                case ApplicationStatus.Pending:
                    return (false, new Response
                    {
                        Message = "The student's application is still under review. Please try again once it has been approved."
                    });

                case ApplicationStatus.Rejected:
                    return (false, new Response
                    {
                        Message = "Sorry, the student's application was rejected and cannot be linked at this time."
                    });

                case ApplicationStatus.Approved:
                    break;
            }

            var parent = new Parent
            {
                Email = request.Email,
                FullName = request.FullName,
                UserName = request.Email.Split('@')[0],
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth
            };

            var result = await _userManager.CreateAsync(parent, request.Password);
            if (!result.Succeeded)
            {
                return (false, new ErrorResponse
                {
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                    Message = "Registration failed."
                });
            }

            await _userManager.AddToRoleAsync(parent, "Parent");

            var token = Guid.NewGuid().ToString("N");
            var linkRequest = new ParentLink
            {
                Token = token,
                ParentId = parent.Id,
                StudentEmail = student.Email,
                ExpiresAt = DateTime.UtcNow.AddDays(3),
                IsUsed = false,
                IsApproved = false,
                RelationshipWithStudent = request.RelationshipWithStudent
            };
            await _parentLinkRepository.AddAsync(linkRequest);

            var approvalUrl = _urlHelper.BuildUrl($"/api/Student/Account/ApproveParentLink?token={token}");
            await _notificationService.SendParentLinkRequestAsync(parent, student, request.RelationshipWithStudent, approvalUrl);

            return (true, new SuccessResponse
            {
                Token = parent.Email,
                Message = "Registration successful. Please check your inbox to verify your email."
            });
        }

        public async Task<(bool Success, Response Response)> RegisterParentWithInviteAsync(ParentRegisterWithInviteRequest request)
        {
            var invite = (await _parentInviteRepository.GetAllAsync())
                .FirstOrDefault(i => i.Token == request.Token);

            if (invite == null || invite.IsUsed || invite.ExpiresAt < DateTime.UtcNow)
                return (false, new Response { Message = "This invitation link is invalid or has expired." });

            var existingUser = await _userManager.FindByEmailAsync(invite.GuardianEmail);
            if (existingUser != null)
                return (false, new Response { Message = "An account with this email already exists." });

            // Create the parent user
            var parent = new Parent
            {
                Email = invite.GuardianEmail,
                FullName = request.FullName,
                UserName = invite.GuardianEmail.Split('@')[0],
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth
            };

            var result = await _userManager.CreateAsync(parent, request.Password);
            if (!result.Succeeded)
            {
                return (false, new ErrorResponse
                {
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                    Message = "Registration failed."
                });
            }

            await _userManager.AddToRoleAsync(parent, "Parent");

            // Mark invite as used
            invite.IsUsed = true;
            await _parentInviteRepository.UpdateAsync(invite);

            // Link parent to student
            var student = (await _userManager.Users.OfType<Student>().ToListAsync())
                .FirstOrDefault(s => s.Id == invite.StudentId);

            if (student == null)
                return (false, new Response { Message = "The student associated with this invite could not be found." });

            var studentParent = new StudentParent
            {
                StudentId = student.Id,
                ParentId = parent.Id,
                RelationshipWithStudent = request.RelationshipWithStudent,
                LinkedAt = DateTime.UtcNow,
                IsConfirmed = true
            };

            await _studentParentRepository.AddAsync(studentParent);

            // Generate token for email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(parent);
            var escapedToken = Uri.EscapeDataString(token);

            var confirmationRelativePath = $"/api/Identity/Account/ConfirmEmail?token={escapedToken}&userID={parent.Id}";
            var emailConfirmationURL = _urlHelper.BuildUrl(confirmationRelativePath);

            // Send emails
            await _notificationService.SendParentLinkedAsync(student, parent, request.RelationshipWithStudent);
            await _notificationService.SendStudentLinkedAsync(parent, student, emailConfirmationURL);

            return (true, new SuccessResponse
            {
                Token = parent.Email,
                Message = "Registration successful. Please check your inbox to verify your email."
            });
        }
    }
}
