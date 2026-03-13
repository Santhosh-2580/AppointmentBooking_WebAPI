using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.Patient;
using AppointmentBooking.Application.InputModels;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Services.Interface;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AppointmentBooking.Web.Controllers.v2
{
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected APIResponse _response;
        private readonly ILogger<UserController> _logger;

        public UserController(IAuthService authService, ILogger<UserController> logger)
        {
            _authService = authService;
            _response = new APIResponse();
            _logger = logger;
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
                    var errors = ModelState.Values
                                  .SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage);

                    foreach (var error in errors)
                    {
                        _response.AddError(error);
                    }
                    _response.DisplayMessage = CommonMessages.RegistrationFailed;
                    _logger.LogWarning("Invalid model state for patient registration: {ModelState}", ModelState);
                    return _response;
                }
                else
                {
                    var createdPatient = await _authService.Register(reg);

                    _response.StatusCode = HttpStatusCode.Created;
                    _response.DisplayMessage = CommonMessages.RegistrationSuccess;
                    _response.IsSuccess = true;                  
                    _response.Result = createdPatient;
                    _logger.LogInformation("Patient registered successfully: {Email}", reg.FirstName);
                    return Ok(_response);
                }

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.RegistrationFailed;
                 _logger.LogError("An error occurred during patient registration for email: {Email}", reg.FirstName);
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);

            }
           
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
                    var errors = ModelState.Values
                                  .SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage);

                    foreach (var error in errors)
                    {
                        _response.AddError(error);
                    }

                    _response.DisplayMessage = CommonMessages.LoginFailed;
                        _logger.LogWarning("Invalid model state for user login: {ModelState}", ModelState);
                    return StatusCode((int)HttpStatusCode.InternalServerError, _response);
                    
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
                    _logger.LogInformation("User logged in successfully: {Email}", login.Email);
                    return Ok(_response);
                }

            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.DisplayMessage = CommonMessages.LoginFailed;
                _logger.LogError("An error occurred during user login for email: {Email}", login.Email);
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
          
        }
    }
}
