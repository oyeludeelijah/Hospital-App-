using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HospitalApp.Web.HospitalApi.Models;
using HospitalApp.Web.HospitalApi.Models.DTOs;
using HospitalApp.Web.HospitalApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace HospitalApp.Web.HospitalApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(UserRegisterDto userDto);
        Task<AuthResponseDto> Login(UserLoginDto userDto);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> Register(UserRegisterDto userDto)
        {
            Console.WriteLine($"Registration attempt for user: {userDto.Username}");
            
            // Initialize response
            var response = new AuthResponseDto();
            
            try
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
                    {
                        response.Success = false;
                        response.Message = "Username and password are required";
                        Console.WriteLine("Registration failed: Username or password empty");
                        return response;
                    }
                    
                    // Check if username already exists
                    if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
                    {
                        response.Success = false;
                        response.Message = "Username already exists";
                        Console.WriteLine("Registration failed: Username already exists");
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Registration error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    
                    // Return error response
                    response.Success = false;
                    response.Message = "An error occurred during registration";
                    return response;
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
                {
                    response.Success = false;
                    response.Message = "Email already exists";
                    Console.WriteLine("Registration failed: Email already exists");
                    return response;
                }

                // Create new user
                var user = new User
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    PasswordHash = BC.HashPassword(userDto.Password),
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Role = userDto.Role,
                    CreatedAt = DateTime.UtcNow
                };
                
                Console.WriteLine($"Creating user with Role: {userDto.Role}");

                // Add to database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"User created successfully with ID: {user.Id}");

                // Generate token
                response.Success = true;
                response.Message = "User registered successfully";
                response.Token = GenerateJwtToken(user);
                response.UserId = user.Id;
                response.Username = user.Username;
                response.Role = user.Role;

                Console.WriteLine("Registration successful, token generated");
                return response;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Registration error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                // Return error response
                response.Success = false;
                response.Message = "An error occurred during registration";
                return response;
            }
        }

        public async Task<AuthResponseDto> Login(UserLoginDto userDto)
        {
            var response = new AuthResponseDto();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => 
                    u.Username == userDto.Username);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

                if (!BC.Verify(userDto.Password, user.PasswordHash))
                {
                    response.Success = false;
                    response.Message = "Invalid password";
                    return response;
                }

                // Update last login
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Generate token
                var token = GenerateJwtToken(user);

                // Return successful response
                response.Success = true;
                response.Message = "Login successful";
                response.Token = token;
                response.UserId = user.Id;
                response.Username = user.Username;
                response.Role = user.Role;

                Console.WriteLine("Login successful, token generated");
                return response;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Login error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                // Return error response
                response.Success = false;
                response.Message = "An error occurred during login";
                return response;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "DefaultSecretKeyForHospitalApp123!@#"));
            
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "HospitalApp",
                audience: _configuration["Jwt:Audience"] ?? "HospitalAppUser",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
