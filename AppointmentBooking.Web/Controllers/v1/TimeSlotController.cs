using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.TimeSlot;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Numerics;
using System.Security.Claims;

namespace AppointmentBooking.Web.Controllers.v1
{
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;
        protected APIResponse _response;
        public TimeSlotController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
                _response = new APIResponse();
        }

        /// <summary>
        /// Add a new time slot for the authenticated doctor. The doctor can specify the date, start time, end time, and maximum number of patients for the time slot. The method validates the input and returns appropriate responses based on the success or failure of the creation operation.
        /// </summary>
        /// <param name="TimeSlot"></param>
        /// <returns></returns>
        [Authorize(Roles ="Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("Add")]
        public async Task<ActionResult<APIResponse>> AddTimeSlot([FromBody] CreateTimeSlotDto TimeSlot)
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
                    var userId = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);

                    var createdTimeSlot = await _timeSlotService.CreateTimeSlotAsync(TimeSlot,userId);

                    _response.StatusCode = HttpStatusCode.Created;
                    _response.DisplayMessage = CommonMessages.CreateOperationSuccess;
                    _response.IsSuccess = true;
                    _response.Result = createdTimeSlot;
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
        /// Retrieves the authenticated doctor's available time slots.
        /// </summary>
        /// <returns>An ActionResult containing an APIResponse with the list of time slots.</returns>
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("DoctorTimeSlots")]
        public async Task<ActionResult<APIResponse>> GetMyTimeSlots()
        {
            try
            {
                var userId = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);

                var TimeSlots = await _timeSlotService.GetMyTimeSlotsAsync(userId);

                if (TimeSlots == null || !TimeSlots.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                }
                else
                {

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = TimeSlots;
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
        //public async Task<ActionResult<APIResponse>> GetTimeSlotsByFilter(int? doctorId)
        //{
        //    try
        //    {
        //        var TimeSlots = await _timeSlotService.GetTimeSlotsByFilterAsync(doctorId);

        //        _response.StatusCode = HttpStatusCode.OK;
        //        _response.IsSuccess = true;
        //        _response.Result = TimeSlots;
        //    }
        //    catch(Exception)
        //    {
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.AddError(CommonMessages.SystemError);
        //    }

        //    return Ok(_response);
        //}

       /// <summary>
       /// Retrieves all available time slots for doctors.
       /// </summary>
       /// <returns>An API response containing the list of available time slots or an error message.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("AvailableSlots")]       
        public async Task<ActionResult<APIResponse>> GetDoctorsAvailableTimeSlots()
        {
            try
            {
                var TimeSlot = await _timeSlotService.GetAllAvailableTimeSlotsAsync();

                if (TimeSlot == null || !TimeSlot.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = TimeSlot;
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
        /// updates the details of an existing time slot for the authenticated doctor. The doctor can update the date, start time, end time, and maximum number of patients for the time slot. The method validates the input and returns appropriate responses based on the success or failure of the update operation.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="timeslotId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPatch("{timeslotId}/update")]
        public async Task<ActionResult<APIResponse>> UpdateTimeSlot(int timeslotId,UpdateTimeSlotDto dto)
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
                    var userId = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);

                    await _timeSlotService.UpdateTimeSlotAsync(userId, dto, timeslotId);

                    _response.StatusCode = HttpStatusCode.Created;
                    _response.DisplayMessage = CommonMessages.UpdateOperationSuccess;
                    _response.IsSuccess = true;
                }

            }
            catch(Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.UpdateOperationFailed;
                _response.AddError(CommonMessages.SystemError);
            }            

            return Ok(_response);

        }

        /// <summary>
        /// Deletes a time slot identified by the specified slot ID.
        /// </summary>
        /// <param name="slotId">The unique identifier of the time slot to delete.</param>
        /// <returns>An API response indicating the result of the delete operation.</returns>
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{slotId}")]
        public async Task<ActionResult<APIResponse>> DeleteTimeSlot(int slotId)
        {
            try
            {
                if (slotId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.DeleteOperationFailed;
                    _response.AddError("Invalid Slot Id");

                    return BadRequest(_response);
                }

                var slot = await _timeSlotService.GetTimeSlotByIdAsync(slotId);

                if (slot == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                    _response.AddError("Slot not found");

                    return NotFound(_response);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _timeSlotService.DeleteTimeSlotAsync(userId, slotId);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.DisplayMessage = CommonMessages.DeleteOperationSuccess;
                _response.IsSuccess = true;               
                return Ok(_response);

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.DeleteOperationFailed;
                _response.AddError(CommonMessages.SystemError);

                return StatusCode(500, _response);
            }
        }


        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{slotId}")]
        public async Task<ActionResult<APIResponse>> GetTimeSlotById(int slotId)
        {
            try
            {
                if (slotId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.GetRecordFailure;
                    _response.AddError("Invalid Slot Id");

                    return BadRequest(_response); 
                }

                var slot = await _timeSlotService.GetTimeSlotByIdAsync(slotId);

                if (slot == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                    _response.AddError("Slot not found");

                    return NotFound(_response); 
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.DisplayMessage = CommonMessages.GetRecordSuccess;
                _response.IsSuccess = true;
                _response.Result = slot;

                return Ok(_response); 
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.GetRecordFailure;
                _response.AddError(CommonMessages.SystemError);

                return StatusCode(500, _response); 
            }
        }

    }
}
