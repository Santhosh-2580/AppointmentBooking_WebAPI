using AppointmentBooking.Application.DTO.Appointment;
using AppointmentBooking.Application.Helper;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Enums;
using AppointmentBooking.Domain.Models;
using AppointmentBooking.Infrastructure.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.ApplicationServices
{
    public class AppointmentService : IAppointmentService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IDoctorRepository _doctorRepository;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper, ITimeSlotRepository timeSlotRepository, IPatientRepository patientRepository, IDoctorRepository doctorRepository,UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;            
            _timeSlotRepository = timeSlotRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;

            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CancelAppointmentAsync(string userId, int AppointmentId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1️⃣ Get Patient
                var patient = await _patientRepository.GetByUserIdAsync(userId);
                if (patient == null)
                    throw new UnauthorizedAccessException("Patient not found.");

                // 2️⃣ Get Appointment
                var appointment = await _appointmentRepository
                    .GetByIdAsync(a => a.Id == AppointmentId);

                if (appointment == null)
                    throw new KeyNotFoundException("Appointment not found.");

                // 3️⃣ Ensure ownership
                if (appointment.PatientId != patient.Id)
                    throw new UnauthorizedAccessException("You cannot cancel this appointment.");

                // 4️⃣ Validate status
                if (appointment.Status == AppointmentStatus.Cancelled)
                    throw new InvalidOperationException("Appointment already cancelled.");

                if (appointment.Status == AppointmentStatus.Completed)
                    throw new InvalidOperationException("Completed appointment cannot be cancelled.");

                // 5️⃣ Get slot
                var slot = await _timeSlotRepository
                    .GetByIdAsync(ts => ts.Id == appointment.TimeSlotId);

                if (slot == null)
                    throw new KeyNotFoundException("TimeSlot not found.");

                // 6️⃣ Update status
                appointment.Status = AppointmentStatus.Cancelled;

                // 7️⃣ Reduce booked count safely
                if (slot.BookedCount > 0)
                    slot.BookedCount -= 1;

                await _appointmentRepository.UpdateAsync(appointment);
                await _timeSlotRepository.UpdateAsync(slot);

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<AppointmentsDto> CreateAppointmentAsync(CreateAppointmentDto dto, string userId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(userId);
            if (patient == null)
                throw new Exception("Patient not found");

            var timeSlot = await _timeSlotRepository.GetTimeSlotsWithDoctorDetailsAsync(x => x.Id == dto.TimeSlotId);

            if (timeSlot == null)
                throw new Exception("Time slot not found");

            if (timeSlot.SlotDate < DateOnly.FromDateTime(DateTime.Today))
                throw new Exception("Cannot book past slot");

            if (timeSlot.BookedCount >= timeSlot.MaxPatients)
                throw new Exception("Slot is full");

            var alreadyBooked = await _appointmentRepository.ExistsAsync(a =>
                a.TimeSlotId == dto.TimeSlotId &&  
                a.PatientId == patient.Id &&
                a.Status == AppointmentStatus.Booked);

            if (alreadyBooked)
                throw new Exception("Patient already booked this slot");            

            var appointment = new Appointment
            {
                PatientId = patient.Id,
                TimeSlotId = dto.TimeSlotId,
                Status = AppointmentStatus.Booked,
                CreatedOn = DateTime.UtcNow
                
            };           

            timeSlot.BookedCount++;

            await _appointmentRepository.CreateAsync(appointment);
            await _timeSlotRepository.UpdateAsync(timeSlot);

            var doctorName = timeSlot.Doctor.DoctorName;
            var specialty = timeSlot.Doctor.Specialty;

            var user = await _userManager.FindByIdAsync(userId);

            return new AppointmentsDto
            {
                Id = appointment.Id,
                TimeSlotId = timeSlot.Id,
                SlotDate = timeSlot.SlotDate,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
                PatientId = patient.Id,
                PatientName = user.FirstName + " " + user.LastName,
                MobileNumber = patient.MobileNumber,
                Gender = patient.Gender,
                Age = AgeHelper.CalculateAge(patient.DateOfBirth),
                DoctorId = timeSlot.DoctorId,
                DoctorName = doctorName,
                Specialty = specialty,
                Status = appointment.Status
            };           
        }      
              

        public async Task<AppointmentsDto> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository
                   .GetAppointmentDetailsByIdAsync(id);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            return new AppointmentsDto
            {
                Id = appointment.Id,
                DoctorName = appointment.TimeSlot.Doctor.DoctorName,
                Specialty = appointment.TimeSlot.Doctor.Specialty,
                SlotDate = appointment.TimeSlot.SlotDate,
                StartTime = appointment.TimeSlot.StartTime,
                EndTime = appointment.TimeSlot.EndTime,
                
            };
        }       
               

        public Task<List<AppointmentsDto>> GetAppointmentsForUserDashboardAsync(string userId, string role)
        {
            if (role == "Patient")
            {
                return GetAppointmentsForPatientDashboardAsync(userId);
            }
            else if (role == "Doctor")
            {
                return GetAppointmentsForDoctorDashboardAsync(userId);
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid role");
            }

            throw new NotImplementedException();
        }

        private async Task<List<AppointmentsDto>> GetAppointmentsForDoctorDashboardAsync(string userId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor == null)
                throw new UnauthorizedAccessException("Doctor not found.");

            var appointments = await _appointmentRepository.GetUpcomingAppointmentsByDoctorIdAsync(doctor.Id);

            var patientIds = appointments.Select(a => a.Patient.UserId).Distinct().ToList();

            var users = await _userManager.Users.Where(u => patientIds.Contains(u.Id)).ToListAsync();


            return appointments.Select(a => 
            {
                var user = users.FirstOrDefault(u => u.Id == a.Patient.UserId);

                return new AppointmentsDto
                {

                    Id = a.Id,

                    DoctorId = doctor.Id,
                    DoctorName = doctor.DoctorName,
                    Specialty = doctor.Specialty,

                    TimeSlotId = a.TimeSlotId,
                    SlotDate = a.TimeSlot.SlotDate,
                    StartTime = a.TimeSlot.StartTime,
                    EndTime = a.TimeSlot.EndTime,

                    PatientId = a.PatientId,
                    PatientName = user != null ? user.FirstName + " " + user.LastName : "Unknown",
                    MobileNumber = a.Patient.MobileNumber,
                    Gender = a.Patient.Gender,
                    Age = AgeHelper.CalculateAge(a.Patient.DateOfBirth),

                    Status = a.Status
                };
                
            }).ToList();
        }

        private async Task<List<AppointmentsDto>> GetAppointmentsForPatientDashboardAsync(string userId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(userId);
            if (patient == null)
                throw new UnauthorizedAccessException("Patient not found.");

            var appointments = await _appointmentRepository.GetUpcomingAppointmentsforPatientDashboard(patient.Id);
            var user = await _userManager.FindByIdAsync(userId);

            return appointments.Select(a => new AppointmentsDto
            {
                Id = a.Id,

                PatientId = a.PatientId,
                PatientName = user.FirstName + " " + user.LastName,
                MobileNumber = a.Patient.MobileNumber,
                Gender = a.Patient.Gender,
                Age = AgeHelper.CalculateAge(a.Patient.DateOfBirth),

                TimeSlotId = a.TimeSlotId,
                SlotDate = a.TimeSlot.SlotDate,
                StartTime = a.TimeSlot.StartTime,
                EndTime = a.TimeSlot.EndTime,

                DoctorId = a.TimeSlot.DoctorId,
                DoctorName =a.TimeSlot.Doctor.DoctorName,
                Specialty = a.TimeSlot.Doctor.Specialty,             

                Status = a.Status
            }).ToList();
        }

        public async Task RescheduleAppointmentAsync(string userId, RescheduleAppointmentDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Get Patient from logged-in user
                var patient = await _patientRepository.GetByUserIdAsync(userId);
                if (patient == null)
                    throw new UnauthorizedAccessException("Patient not found.");

                // Get Appointment
                var appointment = await _appointmentRepository
                    .GetByIdAsync(a => a.Id == dto.AppointmentId);

                if (appointment == null)
                    throw new KeyNotFoundException("Appointment not found.");

                // Ensure appointment belongs to this patient
                if (appointment.PatientId != patient.Id)
                    throw new UnauthorizedAccessException("You cannot reschedule this appointment.");

                // Only Booked appointments can be rescheduled
                if (appointment.Status != AppointmentStatus.Booked)
                    throw new InvalidOperationException("Only booked appointments can be rescheduled.");
                
                //Prevent rescheduling to same slot
                if (appointment.TimeSlotId == dto.NewTimeSlotId)
                    throw new InvalidOperationException("Already booked in this TimeSlot.");

                //Get Old Slot
                var oldSlot = await _timeSlotRepository
                    .GetByIdAsync(ts => ts.Id == appointment.TimeSlotId);

                if (oldSlot == null)
                    throw new KeyNotFoundException("Old TimeSlot not found.");

                //Get New Slot
                var newSlot = await _timeSlotRepository
                    .GetByIdAsync(ts => ts.Id == dto.NewTimeSlotId);

                if (newSlot == null)
                    throw new KeyNotFoundException("New TimeSlot not found.");

                //Prevent rescheduling to past slot
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                if (newSlot.SlotDate < today)
                    throw new InvalidOperationException("Cannot reschedule to past TimeSlot.");

                //Check capacity
                if (newSlot.BookedCount >= newSlot.MaxPatients)
                    throw new InvalidOperationException("Selected TimeSlot is full.");

                //Update BookedCount safely
                if (oldSlot.BookedCount > 0)
                    oldSlot.BookedCount--;

                newSlot.BookedCount++;

                //Update Appointment
                appointment.TimeSlotId = newSlot.Id;

                //Save changes
                await _appointmentRepository.UpdateAsync(appointment);
                await _timeSlotRepository.UpdateAsync(oldSlot);
                await _timeSlotRepository.UpdateAsync(newSlot);

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task MarkasCompletedAsync(int appointmentId)
        {
            var appointment = await _appointmentRepository
                    .GetByIdAsync(a => a.Id == appointmentId);

            if(appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            if (appointment.Status != AppointmentStatus.Booked)
                throw new InvalidOperationException("Only booked appointments can be completd");

            appointment.Status = AppointmentStatus.Completed;

            await _appointmentRepository.UpdateAsync(appointment);

        }
        
        public async Task<List<AppointmentsDto>> GetAllAppointmentsOfUsersAsync(string userId, string role)
        {
            if (role == "Patient")
            {
                return await GetAllAppointmentsforPatientAsync(userId);
            }
            else if (role == "Doctor")
            {
                return await GetAllAppointmentsForDoctorAsync(userId);
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid role");
            }

            throw new NotImplementedException();
        }

        public async Task<List<AppointmentsDto>> GetAllAppointmentsforPatientAsync(string userId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(userId);
            if (patient == null)
                throw new UnauthorizedAccessException("Patient not found.");

            var appointments = await _appointmentRepository.GetAllAppointmentDetailsforPatientAsync(patient.Id);
            var user = await _userManager.FindByIdAsync(userId);

            return appointments.Select(a => new AppointmentsDto
            {
                Id = a.Id,

                PatientId = a.PatientId,
                PatientName = user.FirstName + " " + user.LastName,
                MobileNumber = a.Patient.MobileNumber,
                Gender = a.Patient.Gender,
                Age = AgeHelper.CalculateAge(a.Patient.DateOfBirth),

                TimeSlotId = a.TimeSlotId,
                SlotDate = a.TimeSlot.SlotDate,
                StartTime = a.TimeSlot.StartTime,
                EndTime = a.TimeSlot.EndTime,

                DoctorId = a.TimeSlot.DoctorId,
                DoctorName = a.TimeSlot.Doctor.DoctorName,
                Specialty = a.TimeSlot.Doctor.Specialty,

                Status = a.Status
            }).ToList();
        }
        private async Task<List<AppointmentsDto>> GetAllAppointmentsForDoctorAsync(string userId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor == null)
                throw new UnauthorizedAccessException("Doctor not found.");

            var appointments = await _appointmentRepository.GetAllAppointmentDetailsforDoctorAsync(doctor.Id);

            var patientIds = appointments.Select(a => a.Patient.UserId).Distinct().ToList();

            var users = await _userManager.Users.Where(u => patientIds.Contains(u.Id)).ToListAsync();


            return appointments.Select(a =>
            {
                var user = users.FirstOrDefault(u => u.Id == a.Patient.UserId);

                return new AppointmentsDto
                {

                    Id = a.Id,

                    DoctorId = doctor.Id,
                    DoctorName = doctor.DoctorName,
                    Specialty = doctor.Specialty,

                    TimeSlotId = a.TimeSlotId,
                    SlotDate = a.TimeSlot.SlotDate,
                    StartTime = a.TimeSlot.StartTime,
                    EndTime = a.TimeSlot.EndTime,

                    PatientId = a.PatientId,
                    PatientName = user != null ? user.FirstName + " " + user.LastName : "Unknown",
                    MobileNumber = a.Patient.MobileNumber,
                    Gender = a.Patient.Gender,
                    Age = AgeHelper.CalculateAge(a.Patient.DateOfBirth),

                    Status = a.Status
                };

            }).ToList();
        }
    }
}
