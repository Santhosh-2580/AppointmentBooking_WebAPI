using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.DTO.Doctor;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;
        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }        

        public async Task DeleteDoctorAsync(int id)
        {
           var doctor = await _doctorRepository.GetByIdAsync(x => x.Id == id);
            await _doctorRepository.DeleteAsync(doctor);
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
           var doctors = await _doctorRepository.GetAllAsync();
            return _mapper.Map<List<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto> GetDoctorByIdAsync(int id)
        {
            var doctor = await  _doctorRepository.GetByIdAsync(x => x.Id == id);
            return _mapper.Map<DoctorDto>(doctor);
        }       

      public async Task<CreateDoctorDto> CreateDoctorAsync(CreateDoctorDto doctorDto)
        {
            var user = new ApplicationUser
            {             
                UserName = doctorDto.Email,
                Email = doctorDto.Email,
                PhoneNumber = doctorDto.MobileNumber,
                FirstName = doctorDto.DoctorName,
                LastName = doctorDto.DoctorName,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, doctorDto.Password);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                else    
                await _userManager.AddToRoleAsync(user, "Doctor");

            var doctorEntity = _mapper.Map<Doctor>(doctorDto);

            doctorEntity.UserId = user.Id;
            
            var createdDoctor = await _doctorRepository.CreateAsync(doctorEntity);
            return _mapper.Map<CreateDoctorDto>(createdDoctor);
        }   

        public async Task UpdateDoctorAsync(int id, UpdateDoctorDto dto)
        {
            var doctor = await _doctorRepository.GetByIdAsync(x => x.Id == id);
            if(doctor == null)
            {
                throw new Exception("Doctor not found");
            }

            if(dto.DoctorName != null)
                doctor.DoctorName = dto.DoctorName;
            if(dto.Specialty != null)
                doctor.Specialty = dto.Specialty;
            if(dto.RegistrationNumber != 0)
                doctor.RegistrationNumber = dto.RegistrationNumber;
            if(dto.ExperienceYears != 0)
                doctor.ExperienceYears = dto.ExperienceYears;
            //if(dto.Email != null)
            //    doctor.Email = dto.Email;
            //if(dto.MobileNumber != null)
            //    doctor.MobileNumber = dto.MobileNumber;


            await _doctorRepository.UpdateAsync(doctor);
        }
    }
}
