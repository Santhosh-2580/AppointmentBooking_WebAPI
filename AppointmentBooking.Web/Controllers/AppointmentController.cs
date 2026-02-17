using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.Appointment;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        protected APIResponse _response;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
            _response = new APIResponse();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<ActionResult<APIResponse>> AddAppointment([FromBody] CreateAppointmentDto Appointment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.CreateOperationFailed;
                    _response.AddError(ModelState.ToString());
                }

                var createdAppointment = await _appointmentService.CreateAppointmentAsync(Appointment);

                _response.StatusCode = HttpStatusCode.Created;
                _response.DisplayMessage = CommonMessages.CreateOperationSuccess;
                _response.IsSuccess = true;
                _response.Result = createdAppointment;
            }
            catch   (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.CreateOperationFailed;
                _response.AddError(CommonMessages.SystemError);
            }           

            return Ok(_response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult> GetAppointments()
        {
            try
            {
                var Appointments = await _appointmentService.GetAllAppointmentsAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = Appointments;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;                
                _response.AddError(CommonMessages.SystemError);
            }

            return Ok(_response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Filter")]
        public async Task<ActionResult> GetAppointmentsByFilter(int? doctorId, int? patientId)
        {
            try
            {
                var Appointments = await _appointmentService.GetAppointmentsByFilterAsync(doctorId, patientId);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = Appointments;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);
            }

            return StatusCode((int)_response.StatusCode, _response);

        }

        [ProducesResponseType(StatusCodes.Status200OK)]      
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

        [ProducesResponseType(StatusCodes.Status200OK)]
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

        [ProducesResponseType(StatusCodes.Status200OK)]
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
