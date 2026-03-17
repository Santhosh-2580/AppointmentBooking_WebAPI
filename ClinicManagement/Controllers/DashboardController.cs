using ClinicManagement.Services.Appointment;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppointmentService _appointmentService;
        public DashboardController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }
        public async Task<IActionResult> DoctorDashboard()
        {
            var appointments = await _appointmentService.GetMyAppointmentsasync();

            return View(appointments);
        }
        public async Task<IActionResult> PatientDashboard()
        {
            var appointments = await _appointmentService.GetMyAppointmentsasync();

            return View(appointments);
        }
    }
}
