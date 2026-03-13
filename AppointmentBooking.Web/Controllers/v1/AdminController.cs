using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.Services.Interface;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace AppointmentBooking.Web.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        protected APIResponse _response;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _response = new APIResponse();
            _logger = logger;
        }       
        /// <summary>
        /// Get doctor details by ID. This functionality is essential for the admin to view and manage individual doctor profiles within the appointment booking system.
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-DoctorById/{doctorId}")]
        public async Task<ActionResult<APIResponse>> GetDoctorById(int doctorId)
        {
            try
            {
                var doctor = await _adminService.GetDoctorByIdAsync(doctorId);
                if (doctor != null)
                {
                    _response.IsSuccess = true;
                    _response.Result = doctor;
                    _response.StatusCode = HttpStatusCode.OK;

                    _logger.LogInformation($"Doctor details for ID {doctorId} retrieved successfully.");
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.AddError($"Doctor with ID {doctorId} not found.");
                    return NotFound(_response);
                }
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError($"An error occurred while retrieving doctor details for ID {doctorId}.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        /// <summary>
        /// Admin can view all doctors in the system.This allows the admin to manage and oversee the doctors available in the appointment booking system.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-All-Doctors")]
        public async Task<ActionResult<APIResponse>> GetAllDoctors()
        {
            try
            {
                var doctors = await _adminService.GetAllDoctorsAsync();
                _response.Result = doctors;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                _logger.LogInformation("All doctors retrieved successfully.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError(ex, "An error occurred while retrieving all doctors.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        /// <summary>
        /// Deactivate a doctor from the system. This is useful for managing doctors who are no longer practicing or temporarily unavailable without permanently deleting their records.
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPatch("Delete-Doctor/{doctorId}")]
        public async Task<ActionResult<APIResponse>> DeleteDoctor(int doctorId)
        {
            try
            {
                var userName = User?.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

                var result = await _adminService.DeactivateDoctorAsync(doctorId);
                if (result)
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = $"Doctor with ID {doctorId} has been deleted successfully.";

                    _logger.LogInformation("Doctor with ID {DoctorId} deactivated successfully by {UserName}", doctorId, userName);
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.AddError($"Doctor with ID {doctorId} not found.");
                    return NotFound(_response);
                }
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError($"An error occurred while deactivating doctor with ID {doctorId}.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-PatientById/{patientId}")]
        public async Task<ActionResult<APIResponse>> GetPatientById(int patientId)
        {
            try
            {
                var patient = await _adminService.GetPatientByIdAsync(patientId);
                if (patient != null)
                {
                    _response.IsSuccess = true;
                    _response.Result = patient;
                    _response.StatusCode = HttpStatusCode.OK;
                    _logger.LogInformation($"Patient details for ID {patientId} retrieved successfully.");

                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.AddError($"Patient with ID {patientId} not found.");
                    return NotFound(_response);
                }
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError($"An error occurred while retrieving patient details for ID {patientId}.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-All-Patients")]
        public async Task<ActionResult<APIResponse>> GetAllPatients()
        {
            try
            {
                var patients = await _adminService.GetAllPatientsAsync();
                _response.Result = patients;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                _logger.LogInformation("All patients retrieved successfully.");
                return Ok(_response);
            }
            catch (Exception )
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError("An error occurred while retrieving all patients.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPatch("Delete-Patient/{patientId}")]
        public async Task<ActionResult<APIResponse>> DeletePatient(int patientId)
        {
            try
            {
                var userName = User?.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

                var result = await _adminService.DeactivatePatientAsync(patientId);
                if (result)
                {
                    _response.IsSuccess = true;
                    _response.Result = $"Patient with ID {patientId} has been deleted successfully.";

                    _response.StatusCode = HttpStatusCode.OK;
                    _logger.LogInformation("Patient with ID {PatientId} deactivated successfully by {UserName}.", patientId,userName);
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.AddError($"Patient with ID {patientId} not found.");
                    return NotFound(_response);
                }
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError($"An error occurred while deactivating patient with ID {patientId}.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }
       

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-All-Appointments")]
        public async Task<ActionResult<APIResponse>> GetAllAppointments()
        {
            try
            {
                var appointments = await _adminService.GetAllAppointmentsAsync();
                _response.Result = appointments;
                _response.IsSuccess = true;

                _response.StatusCode = HttpStatusCode.OK;
                _logger.LogInformation("All appointments retrieved successfully.");
                return Ok(_response);
            }
            catch (Exception )
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError("An error occurred while retrieving all appointments.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-AppointmentsByDate/{date}")]
        public async Task<ActionResult<APIResponse>> GetAppointmentsByDate(DateOnly date)
        {
            try
            {
                var appointments = await _adminService.GetAppointmentsByDateAsync(date);
                _response.Result = appointments;
                _response.IsSuccess = true;

                _response.StatusCode = HttpStatusCode.OK;
                _logger.LogInformation($"Appointments for date {date} retrieved successfully.");
                return Ok(_response);
            }
            catch (Exception )
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError($"An error occurred while retrieving appointments for date {date}.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPatch("Cancel-Appointment/{appointmentId}")]
        public async Task<ActionResult<APIResponse>> CancelAppointment(int appointmentId)
        {
            try
            {
                var userName = User?.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

                await _adminService.CancelAppointmentByAdminAsync(appointmentId);
                _response.IsSuccess = true;
                _response.Result = $"Appointment with ID {appointmentId} has been cancelled successfully.";
                _response.StatusCode = HttpStatusCode.OK;

                 _logger.LogInformation($"Appointment with ID {appointmentId} cancelled successfully by {userName}.");
                return Ok(_response);
            }
            catch (Exception )
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError($"An error occurred while cancelling appointment with ID {appointmentId}.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-TimeSlotsByDoctor/{doctorId}")]
        public async Task<ActionResult<APIResponse>> GetTimeSlotsByDoctor(int doctorId)
        {
            try
            {
                var timeSlots = await _adminService.GetTimeSlotsByDoctorAsync(doctorId);
                _response.Result = timeSlots;
                _response.IsSuccess = true;

                _response.StatusCode = HttpStatusCode.OK;
                _logger.LogInformation($"Time slots for doctor with ID {doctorId} retrieved successfully.");
                return Ok(_response);
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError($"An error occurred while retrieving time slots for doctor with ID {doctorId}.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-All-TimeSlots")]
        public async Task<ActionResult<APIResponse>> GetAllTimeSlots()
        {
            try
            {
                var timeSlots = await _adminService.GetAllTimeSlotsAsync();
                _response.Result = timeSlots;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _logger.LogInformation("All time slots retrieved successfully.");
                return Ok(_response);
            }
            catch (Exception )
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);
                _logger.LogError("An error occurred while retrieving all time slots.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

    }
}
