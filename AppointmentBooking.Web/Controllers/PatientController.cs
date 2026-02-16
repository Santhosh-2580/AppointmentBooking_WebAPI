using AppointmentBooking.Application.DTO.Patient;
using AppointmentBooking.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult> AddPatient([FromBody] CreatePatientDto patient)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPatient = await _patientService.CreatePatientAsync(patient);

            return Ok(new { Message = "Patient added successfully", Patient = createdPatient });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult> GetPatients()
        {
            var patients = await _patientService.GetAllPatientsAsync();

            return Ok(patients);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetPatientById(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);

            if (patient == null)
            {

                return NotFound(new { Message = $"Record not found for Id - {id}" });
            }

            return Ok(patient);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdatePatient(int id, UpdatePatientDto patient)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _patientService.UpdatePatientAsync(id, patient);

            return NoContent();

        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete]
        public async Task<ActionResult> DeletePatient(int id)
        {

            if (id == 0)
            {
                return BadRequest(ModelState);
            }

            var patient = await _patientService.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound(new { Message = $"Record not found for Id - {id}" });
            }

            await _patientService.DeletePatientAsync(id);

            return NoContent();

        }
    }
}

