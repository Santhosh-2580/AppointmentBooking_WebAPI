using AppointmentBooking.Application.InputModels;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Application.ViewModels;
using AppointmentBooking.Infrastructure.Identity;
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

namespace AppointmentBooking.Infrastructure.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private ApplicationUser ApplicationUser;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            ApplicationUser = new();

        }

        public async Task<object> Login(Login login)
        {
            ApplicationUser = await _userManager.FindByEmailAsync(login.Email);

            if (ApplicationUser == null)
            {
                return "Invalid Email Address.";
            }
            var result = await _signInManager.PasswordSignInAsync(ApplicationUser, login.Password, isPersistent: true, lockoutOnFailure: false);

            var isValidCredential = await _userManager.CheckPasswordAsync(ApplicationUser, login.Password);

            if (result.Succeeded)
            {
                var token = await GenerateToken();

                LoginResponse loginResponse = new LoginResponse
                {
                    Token = token,
                    UserId = ApplicationUser.Id
                };
                return loginResponse;
            }
            else
            {
                if (result.IsLockedOut)
                {
                    return "Your account is locked out.";
                }
                else if (result.IsNotAllowed)
                {
                    return "You are not allowed to login.";
                }
                else if (result.RequiresTwoFactor)
                {
                    return "Two-factor authentication is required.";
                }
                else if (!isValidCredential)
                {
                    return "Invalid Password.";
                }
                else
                {
                    return "Login failed. Please try again.";
                }
            }
        }

        public async Task<IEnumerable<IdentityError>> Register(Register register)
        {
            ApplicationUser.FirstName = register.FirstName;
            ApplicationUser.LastName = register.LastName;
            ApplicationUser.Email = register.Email;
            ApplicationUser.UserName = register.Email;

            var result = await _userManager.CreateAsync(ApplicationUser, register.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(ApplicationUser, "Patient");
                return null;
            }
            else
            {
                return result.Errors;
            }
        }
        public async Task<string> GenerateToken()
        {
            try
            {
                // 1️. Create Security Key
                // This key is used to SIGN the token.
                // Without this key, token cannot be validated.
                var securityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["JwtSettings:Key"])
                );

                // 2️. Create Signing Credentials
                // Defines which algorithm is used to sign the token.
                // HmacSha256 = industry standard symmetric encryption.
                var signingCredentials = new SigningCredentials(
                    securityKey, SecurityAlgorithms.HmacSha256
                );

                // 3️. Get Roles of the User from ASP.NET Identity
                // Example: Patient, Admin, Doctor
                var roles = await _userManager.GetRolesAsync(ApplicationUser);

                // 4️. Convert Roles into Claims
                // Role claims help in [Authorize(Roles = "Patient")] checks.
                var roleClaims = roles
                    .Select(role => new Claim(ClaimTypes.Role, role))
                    .ToList();

                // 5️. Create Basic Claims for the Token
                // Claims are pieces of information stored inside JWT.
                var claims = new List<Claim>
        {
            // 🔥 VERY IMPORTANT
            // This stores the UserId (Primary Key from AspNetUsers table).
            // Later we retrieve this using:
            // User.FindFirstValue(ClaimTypes.NameIdentifier)
            new Claim(ClaimTypes.NameIdentifier, ApplicationUser.Id),

            // Stores user's email in token
            new Claim(ClaimTypes.Email, ApplicationUser.Email),

            // Stores username
            new Claim(ClaimTypes.Name, ApplicationUser.UserName)
        };

                // Add Role claims to main claims list
                claims.AddRange(roleClaims);

                // 6️. Create the JWT Token
                var token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],      // Who created the token
                    audience: _config["JwtSettings:Audience"],  // Who can use the token
                    claims: claims,                             // User information stored
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["JwtSettings:ExpireMinutes"])), // Token expiry time
                    signingCredentials: signingCredentials      // Signature
                );

                // 7️. Convert Token Object into String
                // This string is sent to frontend.
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Token generation failed: {ex.Message}");
            }
        }
    }
}
