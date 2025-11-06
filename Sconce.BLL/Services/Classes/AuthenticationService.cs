using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Sconce.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Sconce.BLL.Services.Classes
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IFileUrlHelper _fileUrlHelper;
        private readonly IParentInviteRepository _parentInviteRepository;
        private readonly IParentLinkRepository _parentLinkRepository;
        private readonly IStudentParentRepository _studentParentRepository;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            INotificationService notificationService,
            IFileUrlHelper fileUrlHelper,
            IParentInviteRepository parentInviteRepository,
            IParentLinkRepository parentLinkRepository,
            IStudentParentRepository studentParentRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _notificationService = notificationService;
            _fileUrlHelper = fileUrlHelper;
            _parentInviteRepository = parentInviteRepository;
            _parentLinkRepository = parentLinkRepository;
            _studentParentRepository = studentParentRepository;
        }

        public async Task<UserResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user is null)
                throw new Exception("Invalid Email or Password");
            if(!await _userManager.IsEmailConfirmedAsync(user))
                throw new Exception("Please Confirm Your Email");
            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                throw new Exception("Invalid Email or Password");
            return new UserResponse()
            {
                Token = await GenerateTokenAsync(user)
            };
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
            foreach(var role in roles)
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

        public async Task<UserResponse> RegisterStudentAsync(StudentRegisterRequest registerRequest)
        {
            var student = new Student
            {
                Email = registerRequest.Email,
                FullName = registerRequest.FullName,
                UserName = registerRequest.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(student, registerRequest.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(student, "Student");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(student);
            var escapedToken = Uri.EscapeDataString(token);
            var relativePath = $"/api/Identity/Account/ConfirmEmail?token={escapedToken}&userId={student.Id}";
            var confirmationUrl = _fileUrlHelper.BuildFileUrl(relativePath);

            await _notificationService.SendConfirmEmailAsync(student, confirmationUrl);

            return new UserResponse { Token = student.Email };
        }

        public async Task<UserResponse> RegisterParentAsync(ParentRegisterRequest request)
        {
            var student = await _userManager.Users
            .OfType<Student>()
            .FirstOrDefaultAsync(s => s.Email == request.StudentEmail);

            if (student == null)
                throw new InvalidOperationException("No student found with the provided email.");

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
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

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

            var approvalUrl = _fileUrlHelper.BuildFileUrl($"/api/Student/Account/ApproveParentLink?token={token}");
            await _notificationService.SendParentLinkRequestAsync(parent, student, request.RelationshipWithStudent, approvalUrl);

            return new UserResponse { Token = parent.Email };
        }

        public async Task<(bool Success, string Message)> ApproveParentLinkAsync(string token)
        {
            // Validate the link
            var link = (await _parentLinkRepository.GetAllAsync())
                .FirstOrDefault(l => l.Token == token && !l.IsUsed && l.ExpiresAt > DateTime.UtcNow);

            if (link == null)
                return (false, "Invalid or expired token.");

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
                return (false, "Parent or student not found.");

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
            var emailConfirmationURL = _fileUrlHelper.BuildFileUrl(confirmationRelativePath);

            // Send notification emails
            await _notificationService.SendParentLinkedAsync(student, parent, link.RelationshipWithStudent);
            await _notificationService.SendStudentLinkedAsync(parent, student, emailConfirmationURL);

            return (true, "Parent link approved successfully!");
        }

        public async Task<UserResponse> RegisterParentWithInviteAsync(ParentRegisterWithInviteRequest request)
        {
            var invite = (await _parentInviteRepository.GetAllAsync())
                .FirstOrDefault(i => i.Token == request.Token);

            if (invite == null || invite.IsUsed || invite.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("This invitation link is invalid or has expired.");

            var existingUser = await _userManager.FindByEmailAsync(invite.GuardianEmail);
            if (existingUser != null)
                throw new InvalidOperationException("An account with this email already exists.");

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
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(parent, "Parent");

            // Mark invite as used
            invite.IsUsed = true;
            await _parentInviteRepository.UpdateAsync(invite);

            // Link parent to student
            var student = (await _userManager.Users.OfType<Student>().ToListAsync())
                .FirstOrDefault(s => s.Id == invite.StudentId);

            if (student == null)
                throw new InvalidOperationException("The student associated with this invite could not be found.");

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
            var emailConfirmationURL = _fileUrlHelper.BuildFileUrl(confirmationRelativePath);

            await _notificationService.SendParentWelcomeAsync(parent, emailConfirmationURL);

            // Send emails
            await _notificationService.SendParentLinkedAsync(student, parent, request.RelationshipWithStudent);
            await _notificationService.SendStudentLinkedAsync(parent, student, emailConfirmationURL);

            return new UserResponse { Token = parent.Email };
        }

        public async Task<string> ConfirmEmail(string token, string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            if (user is null) throw new Exception("User not Found");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded) return "Email Confirmed Successfully";
            return "Email Confirmation Failed";
        }

        public async Task<string> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordRequest.Email);
            if(user is null) throw new Exception("User not Found");
            var random = new Random();
            var code = random.Next(1000, 9999).ToString();
            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiration = DateTime.UtcNow.AddMinutes(15);
            await _userManager.UpdateAsync(user);
            await _notificationService.SendPasswordResetCodeAsync(forgotPasswordRequest, code);
            return "Please check your email";
        }

        public async Task<string> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user is null) throw new Exception("User not Found");
            if (user.PasswordResetCode != resetPasswordRequest.Code) return "Wrong Code";
            if (user.PasswordResetCodeExpiration < DateTime.UtcNow) return "Code Expired";
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordRequest.NewPassword);
            if (result.Succeeded) await _notificationService.SendPasswordResetSuccessAsync(resetPasswordRequest, user);
            return "Paswword Reset Successfully";
        }
    }
}
