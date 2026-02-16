using AppointmentBooking.Application.DTO.Appointment;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult> AddAppointment([FromBody] CreateAppointmentDto Appointment)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAppointment = await _appointmentService.CreateAppointmentAsync(Appointment);

            return Ok(new { Message = "Appointment added successfully", Appointment = createdAppointment });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult> GetAppointments()
        {
            var Appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(Appointments);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Filter")]
        public async Task<ActionResult> GetAppointmentsByFilter(int? doctorId, int? patientId)
        {
            var Appointments = await _appointmentService.GetAppointmentsByFilterAsync(doctorId,patientId);
            return Ok(Appointments);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetAppointmentById(int id)
        {
            var Appointment = await _appointmentService.GetAppointmentByIdAsync(id);

            if (Appointment == null)
            {

                return NotFound(new { Message = $"Record not found for Id - {id}" });
            }

            return Ok(Appointment);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateAppointment(int id, UpdateAppointmentDto doctor)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _appointmentService.UpdateAppointmentAsync(id, doctor);

            return NoContent();

        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete]
        public async Task<ActionResult> DeleteAppointment(int id)
        {

            if (id == 0)
            {
                return BadRequest(ModelState);
            }

            var doctor = await _appointmentService.GetAppointmentByIdAsync(id);

            if (doctor == null)
            {
                return NotFound(new { Message = $"Record not found for Id - {id}" });
            }

            await _appointmentService.DeleteAppointmentAsync(id);

            return NoContent();

        }

    }
}
