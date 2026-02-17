using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.TimeSlot;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Numerics;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;
        protected APIResponse _response;
        public TimeSlotController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
                _response = new APIResponse();
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
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

                var createdTimeSlot = await _timeSlotService.CreateTimeSlotAsync(TimeSlot);

                _response.StatusCode = HttpStatusCode.Created;
                _response.DisplayMessage = CommonMessages.CreateOperationSuccess;
                _response.IsSuccess = true;
                _response.Result = createdTimeSlot;
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
        public async Task<ActionResult<APIResponse>> GetTimeSlots()
        {
            try
            {
                var TimeSlots = await _timeSlotService.GetTimeSlotsAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = TimeSlots;
            }
            catch(Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;                
                _response.AddError(CommonMessages.SystemError);                
            }            

            return Ok(_response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("Filter")]
        public async Task<ActionResult<APIResponse>> GetTimeSlotsByFilter(int? doctorId)
        {
            try
            {
                var TimeSlots = await _timeSlotService.GetTimeSlotsByFilterAsync(doctorId);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = TimeSlots;
            }
            catch(Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);
            }
            
            return Ok(_response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]     
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<APIResponse>> GetTimeSlotById(int id)
        {
            try
            {
                var TimeSlot = await _timeSlotService.GetTimeSlotByIdAsync(id);

                if (TimeSlot == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = TimeSlot;

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);
            }            

            return Ok(_response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPatch("{id}")]
        public async Task<ActionResult<APIResponse>> UpdateTimeSlot(int id, UpdateTimeSlotDto doctor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.UpdateOperationFailed;
                    _response.AddError(ModelState.ToString());
                }

                await _timeSlotService.UpdateTimeSlotAsync(id, doctor);

                _response.StatusCode = HttpStatusCode.Created;
                _response.DisplayMessage = CommonMessages.UpdateOperationSuccess;
                _response.IsSuccess = true;
            }
            catch(Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.UpdateOperationFailed;
                _response.AddError(CommonMessages.SystemError);
            }            

            return Ok(_response);

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete]
        public async Task<ActionResult<APIResponse>> DeleteTimeSlot(int id)
        {

            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.DeleteOperationFailed;
                    _response.AddError(ModelState.ToString());
                }

                var doctor = await _timeSlotService.GetTimeSlotByIdAsync(id);

                if (doctor == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                    _response.AddError(ModelState.ToString());
                }

                await _timeSlotService.DeleteTimeSlotAsync(id);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.DisplayMessage = CommonMessages.DeleteOperationSuccess;
                _response.IsSuccess = true;

            }
            catch(Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.DeleteOperationFailed;
                _response.AddError(CommonMessages.SystemError);
            }           

            return Ok(_response);

        }

    }
}
