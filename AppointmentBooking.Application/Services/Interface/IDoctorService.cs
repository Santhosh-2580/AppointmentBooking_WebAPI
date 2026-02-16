using AppointmentBooking.Application.DTO.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Services.Interface
{
    public interface IDoctorService
    {
        Task<DoctorDto> GetDoctorByIdAsync(int id);
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();       
        Task<CreateDoctorDto> CreateDoctorAsync(CreateDoctorDto doctorDto);
        Task UpdateDoctorAsync(int id, UpdateDoctorDto updateDoctorDto);
        Task DeleteDoctorAsync(int id);
    }
}
