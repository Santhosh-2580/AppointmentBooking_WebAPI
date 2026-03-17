using AppointmentBooking.Application.DTO.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Services.Interface
{
    public interface IPatientService
    {
        Task<PatientDto> GetPatientProfileAsync(string userId);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto> GetPatientByIdAsync(int id);
        Task<PatientDto> CreatePatientAsync(CreatePatientProfileDto patientDto, string userId);
        Task UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto);
        Task DeletePatientAsync(int id);
       
    }
}
