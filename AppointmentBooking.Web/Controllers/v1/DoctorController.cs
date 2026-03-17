using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.Doctor;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;


namespace AppointmentBooking.Web.Controllers.v1
{
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]

    public class DoctorController : ControllerBase
    {

        private readonly IDoctorService _doctorService;
        protected APIResponse _response;
        private readonly ILogger<AdminController> _logger;

        public DoctorController(IDoctorService doctorService, ILogger<AdminController> logger)
        {
            _doctorService = doctorService;
            _response = new APIResponse();
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]       
        [HttpPost("Add")]
        public async Task<ActionResult<APIResponse>> AddDoctor([FromBody] CreateDoctorDto doctor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.CreateOperationFailed;
                    _response.AddError(ModelState.ToString());     
                    _logger.LogWarning("Invalid model state for creating doctor: {ModelState}", ModelState);
                    return _response;
                }

                else
                {
                    var userName = User?.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

                    var createdDoctor = await _doctorService.CreateDoctorAsync(doctor);

                    _response.StatusCode = HttpStatusCode.Created;
                    _response.DisplayMessage = CommonMessages.CreateOperationSuccess;
                    _response.IsSuccess = true;
                    _response.Result = createdDoctor;
                    _logger.LogInformation("Doctor created successfully with ID: {DoctorId} by {userName}", createdDoctor.Id, userName);
                    return Ok(_response);
                }                

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.CreateOperationFailed;
                _response.AddError(CommonMessages.SystemError);
                _logger.LogError("An error occurred while creating doctor: {ErrorMessage}", CommonMessages.SystemError);
                return _response;
            }
          
        }

        /// <summary>
        /// view all doctors. This endpoint is accessible to both admin and regular users.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Doctors")]
        public async Task<ActionResult<APIResponse>> GetDoctors()
        {
            try
            {

                var doctors = await _doctorService.GetAllDoctorsAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = doctors;
                _logger.LogInformation("Retrieved all doctors successfully. Total count: {DoctorCount}", doctors.Count());
                return Ok(_response);
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);
                _logger.LogError("An error occurred while retrieving doctors: {ErrorMessage}", CommonMessages.SystemError);
                 return Ok(_response);
            }
           
        }

        /// <summary>
        /// View doctor details by id. This endpoint is accessible to both admin and regular users.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<APIResponse>> GetDoctorById(int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);

                if (doctor == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                    _response.AddError(ModelState.ToString());
                    _logger.LogWarning("Doctor with ID: {DoctorId} not found.", id);
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = doctor;
                    _logger.LogInformation("Retrieved doctor details successfully for ID: {DoctorId}", id);
                    return Ok(_response);
                }

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);
                _logger.LogError("An error occurred while retrieving doctor details for ID: {DoctorId}. Error: {ErrorMessage}", id, CommonMessages.SystemError);
                return Ok(_response);
            }           

        }

        /// <summary>
        /// Update doctor details. This endpoint is accessible to both admin and doctors, but doctors can only update their own details, while admins can update any doctor's details.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="doctor"></param>
        /// <returns></returns>
        //[Authorize(Roles = "Doctor,Admin")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[HttpPatch("{id}")]
        //public async Task<ActionResult<APIResponse>> UpdateDoctor(int id, UpdateDoctorDto doctor)
        //{

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.DisplayMessage = CommonMessages.UpdateOperationFailed;
        //            _response.AddError(ModelState.ToString());
        //        }
        //        else
        //        {
        //            await _doctorService.UpdateDoctorAsync(id, doctor);

        //            _response.StatusCode = HttpStatusCode.Created;
        //            _response.DisplayMessage = CommonMessages.UpdateOperationSuccess;
        //            _response.IsSuccess = true;
        //        }                
                
        //    }
        //    catch (Exception)
        //    {
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.DisplayMessage = CommonMessages.UpdateOperationFailed;
        //        _response.AddError(CommonMessages.SystemError);
        //    }

        //    return Ok(_response);

        //}

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<APIResponse>> DeleteDoctor(int id)
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
        //            var doctor = await _doctorService.GetDoctorByIdAsync(id);

        //            if (doctor == null)
        //            {
        //                _response.StatusCode = HttpStatusCode.NotFound;
        //                _response.DisplayMessage = CommonMessages.RecordNotFound;
        //                _response.AddError(ModelState.ToString());
        //            }
        //            else
        //            {
        //                await _doctorService.DeleteDoctorAsync(id);

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
    }
}
