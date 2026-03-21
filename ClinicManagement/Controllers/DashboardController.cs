using ClinicManagement.Services.Appointment;
using ClinicManagement.Services.Patient;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly PatientService _patientService;
        public DashboardController(AppointmentService appointmentService,PatientService patientService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }
        public async Task<IActionResult> DoctorDashboard()
        {
            var response = await _appointmentService.GetMyAppointmentsasync();

            return View(response.Result);
        }
        public async Task<IActionResult> PatientDashboard()
        {
            var appointmentResponse = await _appointmentService.GetMyAppointmentsasync();

            var patientProfileResponse = await _patientService.GetMyProfile();

            ViewBag.HasProfile = patientProfileResponse != null && patientProfileResponse.IsSuccess && patientProfileResponse.Result != null;            

            return View(appointmentResponse.Result);
        }
    }
}
