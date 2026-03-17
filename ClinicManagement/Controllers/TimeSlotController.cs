using ClinicManagement.DTO.TimeSlot;
using ClinicManagement.Helper;
using ClinicManagement.Services.TimeSlot;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers
{
    public class TimeSlotController : Controller
    {
        private readonly TimeSlotService _timeSlotService;

        public TimeSlotController(TimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AvailableSlots()
        {
            var timeslots = await _timeSlotService.GetAvailableSlots();

            return View(timeslots);
        }

        [HttpGet]
        public IActionResult CreateTimeSlot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>CreateSlot(CreateTimeSlotDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _timeSlotService.CreateTimeSlot(model);

            if (response == null)
            {
                ToastHelper.Error(TempData, "Something went wrong!");
                return View(model);
            }

            if (!response.IsSuccess)
            {
                ToastHelper.Error(TempData, response.DisplayMessage);
                return View(model);
            }
            
            ToastHelper.Success(TempData, response.DisplayMessage);

            return View("DoctorTimeSlot");

        }

        [HttpGet]
        public IActionResult DoctorTimeSlot()
        {
            return View();
        }

        public async Task<IActionResult> MyTimeSlots()
        {
            var timeslots = await _timeSlotService.GetAvailableSlots();

            return View("DoctorTimeSlot");
        }
    }
}
