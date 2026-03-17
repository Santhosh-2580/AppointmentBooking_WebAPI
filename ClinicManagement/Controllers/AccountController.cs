using ClinicManagement.Helper;
using ClinicManagement.Models;
using ClinicManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClinicManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.Register(model);

            if (result == null)
            {
                ToastHelper.Error(TempData, "Something went wrong!");               
                return View(model);
            }

            if(!result.IsSuccess)
            {
                ToastHelper.Error(TempData, result.DisplayMessage);
                return View(model);
            }

            ToastHelper.Success(TempData, result.DisplayMessage);
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _authService.Login(model);

            if(response == null)
            {
                ToastHelper.Error(TempData, "Something went wrong!");
                return View(model);
            }

            if(!response.IsSuccess)
            {
                ToastHelper.Error(TempData, response.DisplayMessage);
                return View(model);
            }

            ToastHelper.Success(TempData, response.DisplayMessage);

            var result = response.Result;            

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Token);

            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            HttpContext.Session.SetString("JWToken", result.Token);
            HttpContext.Session.SetString("UserId", result.UserId);
            HttpContext.Session.SetString("Role", role);
            HttpContext.Session.SetString("Name", userName);


            switch (role)
            {
                 case "Admin":
                    return RedirectToAction("AdminDashboard", "Dashboard");
                case "Doctor":
                    return RedirectToAction("DoctorDashboard", "Dashboard");
                case "Patient":
                    return RedirectToAction("PatientDashboard", "Dashboard");
                default:
                    return RedirectToAction("Login");
            }
          
        }
    }
}
