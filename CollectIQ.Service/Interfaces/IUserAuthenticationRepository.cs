using CollectIQ.Core.Dtos;
using CollectIQ.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Service.Interfaces
{
    public interface IUserAuthenticationRepository
    {
        Task<(IdentityResult Result, string userId)> RegisterUserAsync(UserRegistrationDto userForRegistration, CancellationToken ct);

        Task<ValidationResult> ValidateUserAsync(UserLoginDto loginDto);

        Task<string> CreateTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(string userId, string code);

        Task<IdentityResult> ResendConfirmationEmailAsync(string userId);

        Task<IdentityResult> InitiatePasswordResetAsync(string email);

        Task<IdentityResult> ResetPasswordAsync(string email, string code, string password);

        Task<IdentityResult> UpdateUserAsync(UserUpdateDto userUpdateDto, string userId);

        Task LogoutAsync();
    }
}
