using ClinicManagement.DTO.Patient;
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
            var myProfile = await _patientService.GetMyProfile();
            return View(myProfile);
        }

        [HttpGet]
        public IActionResult CreatePatientProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatientProfile(CreatePatientProfileDto model)
        {
            await _patientService.CreateMyProfile(model);
            return RedirectToAction("ViewPatientProfile");
        }
    }
}
