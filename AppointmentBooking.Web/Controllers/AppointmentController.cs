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

                else
                {
                    var createdAppointment = await _appointmentService.CreateAppointmentAsync(Appointment);

                    _response.StatusCode = HttpStatusCode.Created;
                    _response.DisplayMessage = CommonMessages.CreateOperationSuccess;
                    _response.IsSuccess = true;
                    _response.Result = createdAppointment;
                }
                
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.CreateOperationFailed;
                _response.AddError(CommonMessages.SystemError);
            }           

            return Ok(_response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAppointments()
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
        public async Task<ActionResult<APIResponse>> GetAppointmentsByFilter(int? doctorId, int? patientId)
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

            return Ok(_response);

        }

        [ProducesResponseType(StatusCodes.Status200OK)]      
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<APIResponse>> GetAppointmentById(int id)
        {
            try
            {
                var Appointment = await _appointmentService.GetAppointmentByIdAsync(id);

                if (Appointment == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;                   
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = Appointment;
                }                

            }
            catch(Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            

            return Ok(_response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPatch("{id}")]
        public async Task<ActionResult<APIResponse>> UpdateAppointment(int id, UpdateAppointmentDto doctor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.UpdateOperationFailed;
                    _response.AddError(ModelState.ToString());
                }
                else
                {
                    await _appointmentService.UpdateAppointmentAsync(id, doctor);

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.DisplayMessage = CommonMessages.UpdateOperationSuccess;
                    _response.IsSuccess = true;
                }
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.UpdateOperationFailed;
                _response.AddError(CommonMessages.SystemError);
            }            

            return Ok(_response);

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse>> DeleteAppointment(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.DeleteOperationFailed;
                    _response.AddError(ModelState.ToString());
                }

                else
                {
                    var doctor = await _appointmentService.GetAppointmentByIdAsync(id);

                    if (doctor == null)
                    {
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.DisplayMessage = CommonMessages.RecordNotFound;
                        _response.AddError(ModelState.ToString());
                    }
                    else
                    {
                        await _appointmentService.DeleteAppointmentAsync(id);

                        _response.StatusCode = HttpStatusCode.NoContent;
                        _response.DisplayMessage = CommonMessages.DeleteOperationSuccess;
                        _response.IsSuccess = true;
                    }                    
                }                

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.DeleteOperationFailed;
                _response.AddError(CommonMessages.SystemError);
            }            

            return Ok(_response);

        }

    }
}
