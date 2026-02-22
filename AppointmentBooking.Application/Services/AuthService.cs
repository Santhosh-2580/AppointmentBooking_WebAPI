using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.InputModels;
using AppointmentBooking.Application.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ApplicationUser ApplicationUser;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                return true;
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
    }
}
