using AppointmentBooking.Application.DTO.Doctor;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppointmentBooking.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {

        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult> AddDoctor([FromBody] CreateDoctorDto doctor)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdDoctor = await _doctorService.CreateDoctorAsync(doctor);

            return Ok(new { Message = "Doctor added successfully", Doctor = createdDoctor });
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult> GetDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();

            return Ok(doctors);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetDoctorById(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);

            if (doctor == null)
            {

                return NotFound(new { Message = $"Record not found for Id - {id}" });
            }

            return Ok(doctor);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateDoctor(int id, UpdateDoctorDto doctor)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           await _doctorService.UpdateDoctorAsync(id,doctor);

            return NoContent();

        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete]
        public async Task<ActionResult> DeleteDoctor(int id)
        {

            if (id == 0)
            {
                return BadRequest(ModelState);
            }

            var doctor = await _doctorService.GetDoctorByIdAsync(id);

            if (doctor == null)
            {
                return NotFound(new { Message = $"Record not found for Id - {id}" });
            }

           await _doctorService.DeleteDoctorAsync(id);

            return NoContent();

        }
    }
}
