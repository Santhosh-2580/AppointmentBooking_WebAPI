using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.Appointment;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        protected APIResponse _response;
        /// <summary>
        /// Initializes a new instance of the AppointmentController class with the specified appointment service.
        /// </summary>
        /// <param name="appointmentService">Service for managing appointment operations.</param>
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
            _response = new APIResponse();
        }

        /// <summary>
        /// Creates a new appointment using the provided appointment details.
        /// </summary>
        /// <param name="Appointment">The appointment information to create.</param>
        /// <returns>An API response containing the result of the create operation.</returns>
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Create-Appointment")]
        public async Task<ActionResult<APIResponse>> CreateAppointment([FromBody] CreateAppointmentDto Appointment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.AppointmentCreationFailed;
                    _response.AddError(ModelState.ToString());
                }

                else
                {
                    var userId = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
                    var createdAppointment = await _appointmentService.CreateAppointmentAsync(Appointment,userId);

                    _response.StatusCode = HttpStatusCode.Created;
                    _response.DisplayMessage = CommonMessages.AppointmentCreatedSuccess;
                    _response.IsSuccess = true;
                    _response.Result = createdAppointment;
                }
                
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.AppointmentCreationFailed;
                _response.AddError(CommonMessages.SystemError);
            }           

            return Ok(_response);
        }

        /// <summary>
        /// Retrieves the list of appointments associated with the currently authenticated user.
        /// </summary>
        /// <returns>An ActionResult containing an APIResponse with the user's appointments.</returns>
        [Authorize(Roles ="Doctor,Patient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("My-Appointments")]
        public async Task<ActionResult<APIResponse>> GetMyAppointments()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = User.FindFirstValue(ClaimTypes.Role);

                var Appointments = await _appointmentService.GetAppointmentsForUserAsync(userId, role);

                if (Appointments == null || !Appointments.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = Appointments;
                }
                
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;                
                _response.AddError(CommonMessages.SystemError);
            }

            return Ok(_response);
        }

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[HttpGet]
        //[Route("Filter")]
        //public async Task<ActionResult<APIResponse>> GetAppointmentsByFilter(int? doctorId, int? patientId)
        //{
        //    try
        //    {
        //        var Appointments = await _appointmentService.GetAppointmentsByFilterAsync(doctorId, patientId);
        //        _response.StatusCode = HttpStatusCode.OK;
        //        _response.IsSuccess = true;
        //        _response.Result = Appointments;
        //    }
        //    catch (Exception)
        //    {
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.AddError(CommonMessages.SystemError);
        //    }

        //    return Ok(_response);

        //}

        //[ProducesResponseType(StatusCodes.Status200OK)]      
        //[HttpGet]
        //[Route("{id}")]
        //public async Task<ActionResult<APIResponse>> GetAppointmentById(int id)
        //{
        //    try
        //    {
        //        var Appointment = await _appointmentService.GetAppointmentByIdAsync(id);

        //        if (Appointment == null)
        //        {
        //            _response.StatusCode = HttpStatusCode.NotFound;
        //            _response.DisplayMessage = CommonMessages.RecordNotFound;                   
        //        }
        //        else
        //        {
        //            _response.StatusCode = HttpStatusCode.OK;
        //            _response.IsSuccess = true;
        //            _response.Result = Appointment;
        //        }                

        //    }
        //    catch(Exception)
        //    {
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.AddError(CommonMessages.SystemError);
        //        return StatusCode(StatusCodes.Status500InternalServerError, _response);
        //    }


        //    return Ok(_response);
        //}

        /// <summary>
        /// Allows a user to reschedule an existing appointment by providing the appointment ID and the new time slot ID. The user must be authorized as a patient to perform this action.
        /// </summary>
        /// <param name="rescheduleAppointmentDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPatch("Reschedule_Appointment")]
        public async Task<ActionResult<APIResponse>> RescheduleAppointment(RescheduleAppointmentDto rescheduleAppointmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.AppointementRescheduledFailed;
                    _response.AddError(ModelState.ToString());
                }
                else
                {
                    var userId = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
                    await _appointmentService.RescheduleAppointmentAsync(userId, rescheduleAppointmentDto);

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.DisplayMessage = CommonMessages.AppointementRescheduledSuccess;
                    _response.IsSuccess = true;
                }
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.AppointementRescheduledFailed;
                _response.AddError(CommonMessages.SystemError);
            }            

            return Ok(_response);

        }

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<APIResponse>> DeleteAppointment(int id)
        //{
        //    try
        //    {
        //        if (id == 0)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.DisplayMessage = CommonMessages.DeleteOperationFailed;
        //            _response.AddError(ModelState.ToString());
        //        }

        //        else
        //        {
        //            var doctor = await _appointmentService.GetAppointmentByIdAsync(id);

        //            if (doctor == null)
        //            {
        //                _response.StatusCode = HttpStatusCode.NotFound;
        //                _response.DisplayMessage = CommonMessages.RecordNotFound;
        //                _response.AddError(ModelState.ToString());
        //            }
        //            else
        //            {
        //                await _appointmentService.DeleteAppointmentAsync(id);

        //                _response.StatusCode = HttpStatusCode.NoContent;
        //                _response.DisplayMessage = CommonMessages.DeleteOperationSuccess;
        //                _response.IsSuccess = true;
        //            }                    
        //        }                

        //    }
        //    catch (Exception)
        //    {
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.DisplayMessage = CommonMessages.DeleteOperationFailed;
        //        _response.AddError(CommonMessages.SystemError);
        //    }            

        //    return Ok(_response);

        //}

        /// <summary>
        /// Allows a patient to cancel an existing appointment by providing the appointment ID. The user must be authorized as a patient to perform this action.
        /// </summary>
        /// <param name="AppointmentId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Patient")]
        [HttpPatch("{AppointmentId}/Cancel_Appointment")]
        public async Task<IActionResult> CancelAppointment(int AppointmentId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.AppointementCancellationFailed;
                    _response.AddError(ModelState.ToString());
                }
                else
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    await _appointmentService.CancelAppointmentAsync(userId, AppointmentId);

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.DisplayMessage = CommonMessages.AppointementCancelledSuccess;
                    _response.IsSuccess = true;
                }

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.AppointementCancellationFailed;
                _response.AddError(CommonMessages.SystemError);
            }

            return Ok(_response);
        }

    }
}
