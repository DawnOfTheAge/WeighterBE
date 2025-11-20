using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WeighterBE.Data;
using WeighterBE.Data.DTOs;
using WeighterBE.Models;
using WeighterBE.Services;

namespace WeighterBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        ApplicationDbContext context,
        IAuthService authService,
        ILogger<AuthController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthController> _logger = logger;

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest(new { message = "Email already registered" });
                }

                if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return BadRequest(new { message = "Username already taken" });
                }

                // Create new user
                var user = new User
                {
                    Email = request.Email,
                    Username = request.Username,
                    PasswordHash = _authService.HashPassword(request.Password),
                    Role = "User",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("New user registered: {Username}", user.Username);

                // Generate JWT token
                var token = _authService.GenerateJwtToken(user);

                return Ok(new AuthResponse
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        /// <summary>
        /// Login with email/username and password
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Find user by email or username
                var user = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Email == request.EmailOrUsername ||
                        u.Username == request.EmailOrUsername);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                if (!user.IsActive)
                {
                    return Unauthorized(new { message = "Account is inactive" });
                }

                // Verify password
                if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for user: {Username}", user.Username);
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User logged in: {Username}", user.Username);

                // Generate JWT token
                var token = _authService.GenerateJwtToken(user);

                return Ok(new AuthResponse
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Refresh token (optional - extends current token)
        /// </summary>
        [HttpPost("refresh")]
        [Authorize]
        public ActionResult<AuthResponse> RefreshToken()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var username = User.FindFirstValue(ClaimTypes.Name)!;
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var role = User.FindFirstValue(ClaimTypes.Role)!;

                var user = new User
                {
                    Id = userId,
                    Username = username,
                    Email = email,
                    Role = role
                };

                var token = _authService.GenerateJwtToken(user);

                return Ok(new AuthResponse
                {
                    UserId = userId,
                    Username = username,
                    Email = email,
                    Role = role,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }
    }
}