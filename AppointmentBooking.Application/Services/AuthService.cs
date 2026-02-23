using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.InputModels;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Application.ViewModels;
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

namespace AppointmentBooking.Application.Services
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
                if(result.IsLockedOut)
                {
                    return "Your account is locked out.";
                }
                else if(result.IsNotAllowed)
                {
                    return "You are not allowed to login.";
                }
                else if(result.RequiresTwoFactor)
                {
                    return "Two-factor authentication is required.";
                }
                else if(!isValidCredential)
                {
                    return "Invalid Password.";
                }
                else
                {
                    return "Login failed. Please try again.";
                }
            }
        }

        public async Task<string> GenerateToken()
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var roles = await _userManager.GetRolesAsync(ApplicationUser);
                var rolesClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
                List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, ApplicationUser.Email)
        }.Union(rolesClaims).ToList();

                var token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["JwtSettings:ExpireMinutes"])),
                    signingCredentials: signingCredentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Token generation failed: {ex.Message}");
            }
        }
    }
}
