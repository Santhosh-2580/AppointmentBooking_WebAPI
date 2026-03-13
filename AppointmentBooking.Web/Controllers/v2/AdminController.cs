using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.Services.Interface;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace AppointmentBooking.Web.Controllers.v2
{
    [Authorize(Roles = "Admin")]
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
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


        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-Admin-Dashboard")]
        public async Task<ActionResult<APIResponse>> AdminDashboard()
        {
            try
            {
                var dashboardData = await _adminService.GetDashboardAsync();
                _response.IsSuccess = true;
                _response.Result = dashboardData;
                _response.StatusCode = HttpStatusCode.OK;

                _logger.LogInformation("Admin dashboard data retrieved successfully.");
                return Ok(_response);
               
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError("An error occurred while retrieving admin dashboard data.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get-Today-AppointmentsDetails")]
        public async Task<ActionResult<APIResponse>> GetTodayAppointmentsDetails()
        {
            try
            {
                var dashboardData = await _adminService.GetTodayDashboardAsync();
                _response.IsSuccess = true;
                _response.Result = dashboardData;
                _response.StatusCode = HttpStatusCode.OK;

                _logger.LogInformation("Today's appointments details retrieved successfully.");
                return Ok(_response);
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.AddError(CommonMessages.SystemError);

                _logger.LogError("An error occurred while retrieving today's appointments details.");
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        

    }
}
