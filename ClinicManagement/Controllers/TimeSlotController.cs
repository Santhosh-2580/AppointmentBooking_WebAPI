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
            var response = await _timeSlotService.GetAvailableSlots();
            return View(response.Result);
        }

        [HttpGet]
        public IActionResult CreateTimeSlot()
        {
            return View(new CreateUpdateTimeSlotDto
            {
                SlotDate = DateOnly.FromDateTime(DateTime.Today) // ✅ FIX
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSlot(CreateUpdateTimeSlotDto model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateTimeSlot", model);
            }

            var response = await _timeSlotService.CreateTimeSlot(model);

            if (response == null)
            {
                ToastHelper.Error(TempData, "Something went wrong!");
                return View("CreateTimeSlot", model);
            }

            if (!response.IsSuccess)
            {
                ToastHelper.Error(TempData, response.DisplayMessage);
                return View("CreateTimeSlot", model);
            }

            ToastHelper.Success(TempData, response.DisplayMessage);

            // ✅ Redirect to load fresh data
            return RedirectToAction("MyTimeSlots");
        }

       
        public async Task<IActionResult> MyTimeSlots()
        {
            var response = await _timeSlotService.GetMyTimeSlots();

            if (response == null || !response.IsSuccess)
            {
                ToastHelper.Error(TempData, "Unable to load time slots");
                return View("DoctorTimeSlot", new List<TimeSlotsDto>());
            }

            return View("DoctorTimeSlot", response.Result);
        }

        [HttpGet]
        public async Task<IActionResult> EditSlot(int id)
        {
            var response = await _timeSlotService.GetTimeSlotById(id);

            if (response == null || !response.IsSuccess || response.Result == null)
            {
                ToastHelper.Error(TempData, "Unable to load time slot");
                return RedirectToAction("MyTimeSlots");
            }

            // 🔥 Map to Update DTO (if needed)
            var model = new CreateUpdateTimeSlotDto
            {
                Id = response.Result.Id,
                SlotDate = response.Result.SlotDate,
                StartTime = response.Result.StartTime,
                EndTime = response.Result.EndTime,
                MaxPatients = response.Result.MaxPatients
            };
           
            return View("CreateTimeSlot", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSlot(CreateUpdateTimeSlotDto model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateTimeSlot", model);
            }

            // 🔥 Extra validation (important)
            if (model.StartTime >= model.EndTime)
            {
                ModelState.AddModelError("", "Start Time must be less than End Time");
                return View("CreateTimeSlot", model);
            }

            var response = await _timeSlotService.EditSlot(model, model.Id);

            if (response == null)
            {
                ToastHelper.Error(TempData, "Something went wrong!");
                return View("CreateTimeSlot", model);
            }

            if (!response.IsSuccess)
            {
                ToastHelper.Error(TempData, response.DisplayMessage);
                return View("CreateTimeSlot", model);
            }

            ToastHelper.Info(TempData, "Time slot updated successfully");

            return RedirectToAction("MyTimeSlots");
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var response = await _timeSlotService.DeleteSlotId(id);

            if (response == null || !response.IsSuccess)
            {
                ToastHelper.Error(TempData, "Unable to delete time slot");
            }
            else
            {
                ToastHelper.Success(TempData, "Slot deleted.");
            }

            return RedirectToAction("MyTimeSlots");
        }
    }
}
