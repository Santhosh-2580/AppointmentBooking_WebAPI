using AppointmentBooking.Application.DTO.Patient;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto patientDto, string userId)
        {
            var existingPatient = await _patientRepository.GetByUserIdAsync(userId);
            if (existingPatient != null)
            {
                throw new Exception("Patient profile with the same user ID already exists");
            }
            var patientEntity = _mapper.Map<Patient>(patientDto);

            patientEntity.UserId = userId;

            var createdPatient = await _patientRepository.CreateAsync(patientEntity);
            return _mapper.Map<PatientDto>(createdPatient);
        }

        public async Task DeletePatientAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(x => x.Id == id);
            await _patientRepository.DeleteAsync(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            return _mapper.Map<List<PatientDto>>(patients);
        }

        public async Task<PatientDto> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(x => x.Id == id);
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto)
        {
            var patient = await _patientRepository.GetByIdAsync(x => x.Id == id);
            if (patient == null)
            {
                throw new Exception("Patient not found");
            }
            //if(updatePatientDto.PatientName != null)            
            //    patient.PatientName = updatePatientDto.PatientName;
            if(updatePatientDto.MobileNumber != null)
                patient.MobileNumber = updatePatientDto.MobileNumber;
            //if(updatePatientDto.Email != null)
            //    patient.Email = updatePatientDto.Email;
            if(updatePatientDto.DateOfBirth != default)
                patient.DateOfBirth = updatePatientDto.DateOfBirth;
            if (updatePatientDto.Gender != default)
                patient.Gender = updatePatientDto.Gender;

        }
    }
}
