using AppointmentBooking.Application.ApplicationConstants;
using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.Patient;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Models;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        protected APIResponse _response;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
                _response = new APIResponse();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<ActionResult<APIResponse>> AddPatient([FromBody] CreatePatientDto patient)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.CreateOperationFailed;
                    _response.AddError(ModelState.ToString());
                }

                var createdPatient = await _patientService.CreatePatientAsync(patient);

                _response.StatusCode = HttpStatusCode.Created;
                _response.DisplayMessage = CommonMessages.CreateOperationSuccess;
                _response.IsSuccess = true;
                _response.Result = createdPatient;
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
        public async Task<ActionResult<APIResponse>> GetPatients()
        {
            try
            {
                var patients = await _patientService.GetAllPatientsAsync();
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = patients;
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
        public async Task<ActionResult<APIResponse>> GetPatientById(int id)
        {
            try
            {
                var patient = await _patientService.GetPatientByIdAsync(id);

                if (patient == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = patient;
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
        public async Task<ActionResult> UpdatePatient(int id, UpdatePatientDto patient)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.UpdateOperationFailed;
                    _response.AddError(ModelState.ToString());
                }

                await _patientService.UpdatePatientAsync(id, patient);

                _response.StatusCode = HttpStatusCode.Created;
                _response.DisplayMessage = CommonMessages.UpdateOperationSuccess;
                _response.IsSuccess = true;
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
        [HttpDelete]
        public async Task<ActionResult> DeletePatient(int id)
        {

            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.DisplayMessage = CommonMessages.DeleteOperationFailed;
                    _response.AddError(ModelState.ToString());
                }

                var patient = await _patientService.GetPatientByIdAsync(id);

                if (patient == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.DisplayMessage = CommonMessages.RecordNotFound;
                    _response.AddError(ModelState.ToString());
                }

                await _patientService.DeletePatientAsync(id);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.DisplayMessage = CommonMessages.DeleteOperationSuccess;
                _response.IsSuccess = true;

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

