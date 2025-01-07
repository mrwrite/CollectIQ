using AutoMapper;
using CollectIQ.Core.Dtos;
using CollectIQ.Core.Models;
using CollectIQ.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CollectIQ.Service.Services
{
    public class UserAuthenticationRepository : IUserAuthenticationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IEmailManager _emailManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<User> _signInManager;
        private User? _user;

        public UserAuthenticationRepository(
        UserManager<User> userManager,
        IConfiguration configuration,
        IMapper mapper,
        IEmailManager emailManager,
        IHttpContextAccessor httpContextAccessor,
        SignInManager<User> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailManager = emailManager ?? throw new ArgumentNullException(nameof(emailManager));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }


        public async Task<string> CreateTokenAsync(User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public async Task<(IdentityResult Result, string userId)> RegisterUserAsync(UserRegistrationDto userRegistration, CancellationToken ct)
        {
            var user = _mapper.Map<User>(userRegistration);
            var result = await _userManager.CreateAsync(user, userRegistration.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                if (userRegistration.Roles != null && userRegistration.Roles.Length > 0)
                {
                    await _userManager.AddToRolesAsync(user, userRegistration.Roles);
                }

                user.EmailConfirmationSent = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = await GenerateConfirmationLink(user);

                // Use a default value if confirmationLink is null
                confirmationLink ??= "default_confirmation_link";

                var email = new EmailInfo
                {
                    Receiver = user.Email,
                    Sender = "no-reply@mrwrite.dev",
                    Subject = "Welcome to CodeLearn",
                    PlainTextContent = "",
                    HtmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; color: #333;'>
                   <div style='text-align: center; margin-bottom: 30px;'>
                       <a href='https://www.yourwebsite.com'>
                           <img src='{_configuration["LogoUrl"]}' style='width: 100px; margin: 0 auto;' alt='Logo' />
                       </a>
                   </div>
                   <div style='background-color: #f7f7f7; padding: 20px; border-radius: 10px;'>
                       <h2 style='text-align: center; color: #4CAF50;'>Hello, {user.UserName}!</h2>
                       <p style='font-size: 16px; line-height: 24px; margin: 15px 0;'>
                           Welcome to [Your Service Name]! We're excited to have you join us.
                       </p>
                       <p style='font-size: 16px; line-height: 24px; margin: 15px 0;'>
                           To complete your registration and enjoy all the benefits of [Your Service Name], please confirm your email address by clicking the button below:
                       </p>
                       <div style='text-align: center; margin: 30px 0;'>
                           <a href='{confirmationLink}' style='background-color: #4CAF50; color: #fff; text-decoration: none; padding: 15px; border-radius: 5px;'>Confirm your email</a>
                       </div>
                   </div>
                </div>"
                };

                await _emailManager.CreatedAccountEmail(email);
            }

            return (result, user.Id);
        }

        private async Task<string> GenerateConfirmationLink(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/api/auth/confirm-email?userId=" + user.Id + "&token=" + HttpUtility.UrlEncode(token);

            return confirmationLink;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ValidationResult> ValidateUserAsync(UserLoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user != null)
            {
                // Check if the user is currently locked out
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return new ValidationResult { IsValid = false, Message = "This account has been locked out, please try again later." };
                }

                // Validate the password
                if (await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    // If the password is valid, reset the count of failed attempts
                    await _userManager.ResetAccessFailedCountAsync(user);
                    return new ValidationResult { IsValid = true, User = user };
                }
                else
                {
                    // If the password is invalid, increment the count of failed attempts
                    await _userManager.AccessFailedAsync(user);

                    // Check if the failed attempts have reached the maximum attempts defined in the lockout settings
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        return new ValidationResult { IsValid = false, Message = "Too many failed login attempts. This account has been locked out, please try again later." };
                    }

                    return new ValidationResult { IsValid = false, Message = "Invalid password." };
                }
            }
            return new ValidationResult { IsValid = false, Message = "User not found." };
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            if (user.EmailConfirmationSent.HasValue && DateTime.UtcNow - user.EmailConfirmationSent.Value > TimeSpan.FromHours(24))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Confirmation link expired" });
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return result;
        }

        public async Task<IdentityResult> ResendConfirmationEmailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            user.EmailConfirmationSent = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = await GenerateConfirmationLink(user);

            // Use a default value if confirmationLink is null
            confirmationLink ??= "default_confirmation_link";

            var email = new EmailInfo
            {
                Receiver = user.Email,
                Sender = "no-reply@mrwrite.dev",
                Subject = "Welcome back to CodeLearn",
                PlainTextContent = "",
                HtmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; color: #333;'>
                    <div style='text-align: center; margin-bottom: 30px;'>
                        <a href='https://www.yourwebsite.com'>
                            <img src='{_configuration["LogoUrl"]}' style='width: 100px; margin: 0 auto;' alt='Logo' />
                        </a>
                    </div>
                    <div style='background-color: #f7f7f7; padding: 20px; border-radius: 10px;'>
                        <h2 style='text-align: center; color: #4CAF50;'>Hello, {user.UserName}!</h2>
                        <p style='font-size: 16px; line-height: 24px; margin: 15px 0;'>
                            Welcome back to CodeLearn! We're still excited to have you with us.
                        </p>
                        <p style='font-size: 16px; line-height: 24px; margin: 15px 0;'>
                            It seems like your confirmation link expired. Not to worry, you can confirm your email address by clicking the button below:
                        </p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{confirmationLink}' style='background-color: #4CAF50; color: #fff; text-decoration: none; padding: 15px; border-radius: 5px;'>Confirm your email</a>
                        </div>
                    </div>
                </div>"
            };

            await _emailManager.CreatedAccountEmail(email);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> InitiatePasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return IdentityResult.Success;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/api/auth/reset-password?userId=" + user.Id + "&token=" + HttpUtility.UrlEncode(token);

            var reset_email = new EmailInfo
            {
                Receiver = user.Email,
                Sender = "no-reply@mrwrite.dev",
                Subject = "Reset Your Password",
                PlainTextContent = "",
                HtmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; color: #333;'>
                   <div style='text-align: center; margin-bottom: 30px;'>
                       <a href='https://www.yourwebsite.com'>
                           <img src='{_configuration["LogoUrl"]}' style='width: 100px; margin: 0 auto;' alt='Logo' />
                       </a>
                   </div>
                   <div style='background-color: #f7f7f7; padding: 20px; border-radius: 10px;'>
                       <h2 style='text-align: center; color: #4CAF50;'>Hello, {user.UserName}!</h2>
                       <p style='font-size: 16px; line-height: 24px; margin: 15px 0;'>
                           We received a request to reset your password for your [Your Service Name] account.
                       </p>
                       <p style='font-size: 16px; line-height: 24px; margin: 15px 0;'>
                           Click the button below to reset your password. This link will be valid for the next 24 hours.
                       </p>
                       <div style='text-align: center; margin: 30px 0;'>
                           <a href='{callbackUrl}' style='background-color: #4CAF50; color: #fff; text-decoration: none; padding: 15px; border-radius: 5px;'>Reset your password</a>
                       </div>
                       <p style='font-size: 16px; line-height: 24px; margin: 15px 0;'>
                           If you did not request a password reset, please ignore this email or reply to let us know. This password reset is only valid for the next 24 hours.
                       </p>
                   </div>
                </div>"
            };

            await _emailManager.SendEmailAsync(reset_email);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return IdentityResult.Success;
            }

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<IdentityResult> UpdateUserAsync(UserUpdateDto userUpdateDto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            user.Email = userUpdateDto.Email;
            // Update other fields as needed

            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtConfig = _configuration.GetSection("JwtConfig");
            var validIssuer = jwtConfig.GetValue<string>("ValidIssuer");
            var validAudience = jwtConfig.GetValue<string>("ValidAudience");
            var expiresIn = jwtConfig.GetValue<double>("ExpiresIn");

            var tokenOptions = new JwtSecurityToken
            (
                issuer: validIssuer,
                audience: validAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiresIn),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id),
                new Claim("UserName", user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtConfig = _configuration.GetSection("JwtConfig");
            var secretValue = jwtConfig.GetValue<string>("Secret");
            var key = Encoding.UTF8.GetBytes(secretValue);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
    }
}
