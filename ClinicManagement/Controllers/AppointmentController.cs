using ClinicManagement.DTO.Appointment;
using ClinicManagement.DTO.TimeSlot;
using ClinicManagement.Helper;
using ClinicManagement.Services.Appointment;
using ClinicManagement.Services.TimeSlot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly TimeSlotService _timeSlotService;

        public AppointmentController(AppointmentService appointmentService, TimeSlotService timeSlotService)
        {
            _appointmentService = appointmentService;
            _timeSlotService = timeSlotService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(CreateAppointmentDto model)
        {
            var response = await _appointmentService.CreateAppointment(model);

            if (response == null || !response.IsSuccess)
            {
                ToastHelper.Error(TempData, "Booking failed");
            }
            else
            {
                ToastHelper.Success(TempData, "Appointment booked successfully");
            }

            return RedirectToAction("PatientDashboard", "Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var response = await _appointmentService.MarkAsCompletedAsync(id);

            if (response == null || !response.IsSuccess)
            {
                ToastHelper.Error(TempData, response.DisplayMessage ??  "Unable to complete");
                return RedirectToAction("DoctorDashboard", "Dashboard");
            }

            ToastHelper.Success(TempData, response.DisplayMessage ?? "Appointment completed.");
            return RedirectToAction("DoctorDashboard", "Dashboard");
        }
        
        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var response = await _appointmentService.MarkAsCancelAsync(id);

            if (response == null || !response.IsSuccess)
            {
                ToastHelper.Error(TempData, response.DisplayMessage ?? "Unable to cancel");
                return RedirectToAction("PatientDashboard", "Dashboard");
            }

            ToastHelper.Success(TempData, response.DisplayMessage ?? "Appointment canceled.");
            return RedirectToAction("PatientDashboard", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Appointments()
        {
            var response = await _appointmentService.GetAllAppointmentDetailsAsync();

            if (response == null || !response.IsSuccess)
            {
                return View(new List<AppointmentsDto>());
            }

            return View(response.Result);
        }

        [HttpGet]
        public async Task<IActionResult> AppointmentReschedule(int appointmentId)
        {
            // Get appointment details (for showing current info)
            var appointmentResponse = await _appointmentService.GetAppointmentByIdAsync(appointmentId);

            if (appointmentResponse == null || !appointmentResponse.IsSuccess)
            {
                return RedirectToAction("Appointments");
            }

            var appointment = appointmentResponse.Result;

            // Get available slots (same like Available Slots page)
            var slotsResponse = await _timeSlotService.GetAvailableSlots();

            ViewBag.AvailableSlots = slotsResponse.Result;

            // Send current appointment details to UI
            ViewBag.DoctorName = appointment.DoctorName;
            ViewBag.Specialty = appointment.Specialty;

            ViewBag.OldSlotDate = appointment.SlotDate;
            ViewBag.OldStartTime = appointment.StartTime;
            ViewBag.OldEndTime = appointment.EndTime;

            // Prepare model
            var model = new RescheduleAppointmentDto
            {
                AppointmentId = appointmentId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RescheduleAppointment(RescheduleAppointmentDto model)
        {
            if (!ModelState.IsValid)
            {
                // Reload slots if validation fails
                var slotsResponse = await _timeSlotService.GetAvailableSlots();
                ViewBag.AvailableSlots = slotsResponse.Result;

                return View("Reschedule", model);
            }

            var response = await _appointmentService.RescheduleAppointment(model);

            if (response != null && response.IsSuccess)
            {
                ToastHelper.Success(TempData, "Appointment rescheduled successfully!");                
                return RedirectToAction("PatientDashboard", "Dashboard");
            }

            ToastHelper.Error(TempData, "Reschedule failed. Try again.");           

            // Reload slots again
            var slots = await _timeSlotService.GetAvailableSlots();
            ViewBag.AvailableSlots = slots.Result;

            return View("AppointmentReschedule", model);
        }
    }
}
