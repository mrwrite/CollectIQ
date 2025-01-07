using AutoMapper;
using CollectIQ.Core.Dtos;
using CollectIQ.Core.Models;
using CollectIQ.Service.Filters;
using CollectIQ.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CollectIQ.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AuthController(
            IRepositoryManager repository, 
            ILoggerManager logger, 
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<Role> roleManager) : base(repository, logger, mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userRegistration, CancellationToken ct)
        {
            var (userResult, userId) = await _repository.UserAuthentication.RegisterUserAsync(userRegistration, ct);

            if (!userResult.Succeeded)
            {
                return new BadRequestObjectResult(userResult);
            }

            var successMessage = new
            {
                Message = "Registration successful, please check your email for confirmation link."
            };

            return CreatedAtAction(nameof(GetUser), new { id = userId }, successMessage);
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
        {
            var validationResult = await _repository.UserAuthentication.ValidateUserAsync(user);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { error = validationResult.Message });
            }

            if (validationResult == null || !validationResult.User.EmailConfirmed)
            {
                return Unauthorized();
            }

            var token = await _repository.UserAuthentication.CreateTokenAsync(validationResult.User);
            return Ok(new { Token = token });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<UserDto>(user);

            return Ok(result);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var result = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userManager.Users.Skip((pageNumber - 1) * pageSize).
                Take(pageSize).ToListAsync();


            var totalUsers = await _userManager.Users.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var paginationMetadata = new
            {
                TotalCount = totalUsers,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                HasPrevious = pageNumber > 1,
                HasNext = pageNumber < totalPages
            };

            var result = _mapper.Map<IEnumerable<UserDto>>(users);

            return Ok(new
            {
                Metadata = paginationMetadata,
                Users = result
            });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _repository.UserAuthentication.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Message = "Email confirmed successfully."
                });
            }
            else
            {
                if (result.Errors.Any(e => e.Description == "Confirmation link expired"))
                {
                    return BadRequest(new { Message = "Confirmation link has expired. Please resend the confirmation email." });
                }
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _repository.UserAuthentication.LogoutAsync();
            return NoContent();
        }

        [HttpPost("resend-confirmation-email/{userId}")]
        public async Task<IActionResult> ResendConfirmationEmail(string userId)
        {
            var result = await _repository.UserAuthentication.ResendConfirmationEmailAsync(userId);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Confirmation email has been resent." });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _repository.UserAuthentication.InitiatePasswordResetAsync(email);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string userId, string token, string newPassword)
        {
            var result = await _repository.UserAuthentication.ResetPasswordAsync(userId, token, newPassword);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (id != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                return Unauthorized();

            var result = await _repository.UserAuthentication.UpdateUserAsync(userUpdateDto, id);
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
    
    
}
