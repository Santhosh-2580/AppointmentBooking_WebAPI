using AppointmentBooking.Application.DTO.TimeSlot;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;
        public TimeSlotController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }


        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult> AddTimeSlot([FromBody] CreateTimeSlotDto TimeSlot)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTimeSlot = await _timeSlotService.CreateTimeSlotAsync(TimeSlot);

            return Ok(new { Message = "TimeSlot added successfully", TimeSlot = createdTimeSlot });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult> GetTimeSlots()
        {
            var TimeSlots = await _timeSlotService.GetTimeSlotsAsync();
            return Ok(TimeSlots);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Filter")]
        public async Task<ActionResult> GetTimeSlotsByFilter(int? doctorId)
        {
            var TimeSlots = await _timeSlotService.GetTimeSlotsByFilterAsync(doctorId);
            return Ok(TimeSlots);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetTimeSlotById(int id)
        {
            var TimeSlot = await _timeSlotService.GetTimeSlotByIdAsync(id);

            if (TimeSlot == null)
            {

                return NotFound(new { Message = $"Record not found for Id - {id}" });
            }

            return Ok(TimeSlot);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateTimeSlot(int id, UpdateTimeSlotDto doctor)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _timeSlotService.UpdateTimeSlotAsync(id, doctor);

            return NoContent();

        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete]
        public async Task<ActionResult> DeleteTimeSlot(int id)
        {

            if (id == 0)
            {
                return BadRequest(ModelState);
            }

            var doctor = await _timeSlotService.GetTimeSlotByIdAsync(id);

            if (doctor == null)
            {
                return NotFound(new { Message = $"Record not found for Id - {id}" });
            }

            await _timeSlotService.DeleteTimeSlotAsync(id);

            return NoContent();

        }

    }
}
