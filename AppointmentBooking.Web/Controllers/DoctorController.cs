using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.Doctor;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {

        private readonly IDoctorService _doctorService;
        protected APIResponse _response;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
            _response = new APIResponse();
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]       
        [HttpPost]
        public async Task<ActionResult<APIResponse>> AddDoctor([FromBody] CreateDoctorDto doctor)
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
                    var createdDoctor = await _doctorService.CreateDoctorAsync(doctor);

                    _response.StatusCode = HttpStatusCode.Created;
                    _response.DisplayMessage = CommonMessages.CreateOperationSuccess;
                    _response.IsSuccess = true;
                    _response.Result = createdDoctor;
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

        /// <summary>
        /// view all doctors. This endpoint is accessible to both admin and regular users.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("View-all-doctors")]
        public async Task<ActionResult<APIResponse>> GetDoctors()
        {
            try
            {

                var doctors = await _doctorService.GetAllDoctorsAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = doctors;
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);
            }


            return Ok(_response);
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
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = doctor;
                }

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);
            }
            return Ok(_response);

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
