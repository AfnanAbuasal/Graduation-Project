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

namespace Sconce.BLL.Services.Classes
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IFileUrlHelper _fileUrlHelper;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration, INotificationService notificationService, IFileUrlHelper fileUrlHelper)
        {
            _userManager = userManager;
            _configuration = configuration;
            _notificationService = notificationService;
            _fileUrlHelper = fileUrlHelper;
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

        public async Task<UserResponse> RegisterStudentAsync(RegisterRequest registerRequest)
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

        public async Task<UserResponse> RegisterParentAsync(RegisterRequest registerRequest)
        {
            var parent = new Parent
            {
                Email = registerRequest.Email,
                FullName = registerRequest.FullName,
                UserName = registerRequest.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(parent, registerRequest.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(parent, "Parent");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(parent);
            var escapedToken = Uri.EscapeDataString(token);
            var relativePath = $"/api/Identity/Account/ConfirmEmail?token={escapedToken}&userId={parent.Id}";
            var confirmationUrl = _fileUrlHelper.BuildFileUrl(relativePath);

            await _notificationService.SendConfirmEmailAsync(parent, confirmationUrl);

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
