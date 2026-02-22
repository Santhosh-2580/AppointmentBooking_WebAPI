using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.Patient;
using AppointmentBooking.Application.InputModels;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected APIResponse _response;

        public UserController(IAuthService authService)
        {
            _authService = authService;
            _response = new APIResponse();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [Route("Register-patient")]
        public async Task<ActionResult<APIResponse>> RegisterPatient([FromBody] Register reg)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.AddError(ModelState.ToString());
                    _response.DisplayMessage = CommonMessages.RegistrationFailed;
                    return _response;
                }
                else
                {
                    var createdPatient = await _authService.Register(reg);

                    _response.StatusCode = HttpStatusCode.Created;
                    _response.DisplayMessage = CommonMessages.RegistrationSuccess;
                    _response.IsSuccess = true;
                    _response.Result = createdPatient;
                }


            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.RegistrationFailed;
               
            }

            return Ok(_response);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<APIResponse>> Login([FromBody] Login login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.AddError(ModelState.ToString());
                    _response.DisplayMessage = CommonMessages.LoginFailed;
                    return _response;
                }
                else
                {
                    var result = await _authService.Login(login);

                    if (result is string)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.DisplayMessage = $"{result.ToString()} - {CommonMessages.LoginFailed}";
                        return _response;
                    }

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.DisplayMessage = CommonMessages.LoginSuccess;
                    _response.IsSuccess = true;
                    _response.Result = result;
                }

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.LoginFailed;
            }

            return Ok(_response);
        }
    }
}
