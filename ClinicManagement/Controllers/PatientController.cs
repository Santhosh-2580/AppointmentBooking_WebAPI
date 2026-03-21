using ClinicManagement.DTO.Patient;
using ClinicManagement.Helper;
using ClinicManagement.Services.Patient;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientService _patientService;

        public PatientController(PatientService patientService)
        {
            _patientService = patientService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewPatientProfile()
        {
            var response = await _patientService.GetMyProfile();

            if (response == null || response.Result == null)
            {
                ToastHelper.Error(TempData, "Profile not found");                
                return View(null);
            }

            return View(response.Result);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePatientProfile()
        {
            var response = await _patientService.GetMyProfile();
           
            if (response != null && response.IsSuccess && response.Result != null)
            {
                ToastHelper.Info(TempData, "Profile already exists!");
                return RedirectToAction("ViewPatientProfile");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatientProfile(CreatePatientProfileDto model)
        {
            var response = await _patientService.CreateMyProfile(model);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError("", response.DisplayMessage);
                return View(model);
            }

            return RedirectToAction("ViewPatientProfile");
        }
    }
}
