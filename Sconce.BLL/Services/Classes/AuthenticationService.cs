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
        private readonly IEmailSender _emailSender;
        private readonly IFileUrlHelper _fileUrlHelper;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender, IFileUrlHelper fileUrlHelper)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
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

        public async Task<UserResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            var user = new ApplicationUser()
            {
                Email = registerRequest.Email,
                FullName = registerRequest.FullName,
                UserName = registerRequest.UserName,
                PhoneNumber = registerRequest.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var escapedToken = Uri.EscapeDataString(token);

                var confirmationRelativePath = $"/api/Identity/Account/ConfirmEmail?token={escapedToken}&userID={user.Id}";
                var emailConfirmationURL = _fileUrlHelper.BuildFileUrl(confirmationRelativePath);


                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Welcome to Sconce – Confirm Your Email",
                    $@"
                    <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                        <h2 style='color: #2c3e50;'>Welcome to <span style='color: #1abc9c;'>Sconce</span>!</h2>
                        <p>Hi <strong>{user.FullName}</strong>,</p>

                        <p>We’re thrilled to have you join our learning community! To get started, please confirm your email address by clicking the button below:</p>

                        <div style='text-align: center; margin: 25px 0;'>
                            <a href='{emailConfirmationURL}' 
                               style='background-color: #1abc9c; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                               Confirm Email
                            </a>
                        </div>

                        <p>If the button doesn’t work, you can also copy and paste this link into your browser:</p>
                        <p style='word-break: break-all; color: #1abc9c;'>{emailConfirmationURL}</p>

                        <p>Thank you for joining us – we’re excited to see you shine!</p>

                        <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>
                    </div>");

                return new UserResponse()
                {
                    Token = registerRequest.Email //temporary
                };
            } else
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
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
            await _emailSender.SendEmailAsync(forgotPasswordRequest.Email, "Reset Password", $"<p>Your Code for Reseting Password:</p><h1>{code}<h1/>");
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
            if (result.Succeeded) await _emailSender.SendEmailAsync(resetPasswordRequest.Email, "Password Reset Success", $"<h1>Hello, {user.FullName}!<h1/><p>Your password has been updated.<p/>");
            return "Paswword Reset Successfully";
        }
    }
}
