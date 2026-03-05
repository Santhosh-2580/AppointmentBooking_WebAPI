using AppointmentBooking.Application.DTO.Admin;
using AppointmentBooking.Application.DTO.Appointment;
using AppointmentBooking.Application.DTO.Doctor;
using AppointmentBooking.Application.DTO.Patient;
using AppointmentBooking.Application.DTO.TimeSlot;
using AppointmentBooking.Application.Helper;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Enums;
using AppointmentBooking.Infrastructure.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.ApplicationServices
{
    public class AdminService : IAdminService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IAppointmentRepository appointmentRepository, ITimeSlotRepository timeSlotRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _timeSlotRepository = timeSlotRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CancelAppointmentByAdminAsync(int appointmentId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var appointment = await _appointmentRepository.GetByIdAsync(x => x.Id == appointmentId);

                if (appointment == null)
                {
                    throw new InvalidOperationException("Appointment not found.");
                }
                else
                {
                    if (appointment.Status == AppointmentStatus.Cancelled)
                        throw new InvalidOperationException("Appointment already cancelled.");

                    if (appointment.Status == AppointmentStatus.Completed)
                        throw new InvalidOperationException("Completed appointment cannot be cancelled.");

                    var slot = await _timeSlotRepository
                        .GetByIdAsync(ts => ts.Id == appointment.TimeSlotId);

                    if (slot == null)
                        throw new KeyNotFoundException("TimeSlot not found.");

                    // 6️⃣ Update status
                    appointment.Status = AppointmentStatus.Cancelled;

                    if (slot.BookedCount > 0)
                        slot.BookedCount -= 1;

                    await _appointmentRepository.UpdateAsync(appointment);
                    await _timeSlotRepository.UpdateAsync(slot);

                    await _unitOfWork.CommitAsync();
                }
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

        }
        public async Task<IEnumerable<AppointmentsDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.GetAppointmentDetailsAsync();

            var patientIds = appointments.Select(a => a.Patient.UserId).Distinct().ToList();

            var users = await _userManager.Users.Where(u => patientIds.Contains(u.Id)).ToListAsync();

            var userDict = users.ToDictionary(u => u.Id);

            return appointments.Select(a =>
            {
                userDict.TryGetValue(a.Patient.UserId, out var user);

                return new AppointmentsDto
                {
                    Id = a.Id,

                    PatientId = a.PatientId,
                    PatientName = user != null ? user.FirstName + " " + user.LastName : "Unknown",
                    MobileNumber = a.Patient.MobileNumber,
                    Gender = a.Patient.Gender,
                    Age = AgeHelper.CalculateAge(a.Patient.DateOfBirth),

                    DoctorId = a.TimeSlot.DoctorId,
                    DoctorName = a.TimeSlot.Doctor.DoctorName,
                    Specialty = a.TimeSlot.Doctor.Specialty,

                    TimeSlotId = a.TimeSlotId,
                    SlotDate = a.TimeSlot.SlotDate,
                    StartTime = a.TimeSlot.StartTime,
                    EndTime = a.TimeSlot.EndTime,

                    Status = a.Status
                };

            }).ToList();
        }
        public async Task<IEnumerable<AppointmentsDto>> GetAppointmentsByDateAsync(DateOnly date)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDateAsync(date);

            if (appointments == null)
                throw new InvalidOperationException("NO Appointments to display");

            var patientIds = appointments.Select(a => a.Patient.UserId).Distinct().ToList();

            var users = await _userManager.Users.Where(u => patientIds.Contains(u.Id)).ToListAsync();

            var userDict = users.ToDictionary(u => u.Id);
            return appointments.Select(a =>
            {
                userDict.TryGetValue(a.Patient.UserId, out var user);

                return new AppointmentsDto
                {
                    Id = a.Id,

                    PatientId = a.PatientId,
                    PatientName = user != null ? user.FirstName + " " + user.LastName : "Unknown",
                    MobileNumber = a.Patient.MobileNumber,
                    Gender = a.Patient.Gender,
                    Age = AgeHelper.CalculateAge(a.Patient.DateOfBirth),

                    DoctorId = a.TimeSlot.DoctorId,
                    DoctorName = a.TimeSlot.Doctor.DoctorName,
                    Specialty = a.TimeSlot.Doctor.Specialty,

                    TimeSlotId = a.TimeSlotId,
                    SlotDate = a.TimeSlot.SlotDate,
                    StartTime = a.TimeSlot.StartTime,
                    EndTime = a.TimeSlot.EndTime,

                    Status = a.Status
                };

            }).ToList();

        }

        public async Task DeactivatePatientAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(p => p.Id == id);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            if (!patient.IsActive)
                return;

            patient.IsActive = false;
            await _patientRepository.UpdateAsync(patient);
        }
        public async Task<PatientDto> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(p => p.Id == id);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            var user = await _userManager.FindByIdAsync(patient.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return new PatientDto
            {
                Id = patient.Id,
                PatientName = user.FirstName + " " + user.LastName,
                MobileNumber = patient.MobileNumber,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Email = user.Email,

            };
        }
        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetActivePatientsAsync();

            var patientIds = patients.Select(p => p.UserId).ToList();

            var users = await _userManager.Users.Where(u => patientIds.Contains(u.Id)).ToListAsync();

            var userDict = users.ToDictionary(u => u.Id);

            return patients.Select(p =>
            {
                userDict.TryGetValue(p.UserId, out var user);
                return new PatientDto
                {
                    Id = p.Id,
                    PatientName = user != null ? user.FirstName + " " + user.LastName : "Unknown",
                    MobileNumber = p.MobileNumber,
                    DateOfBirth = p.DateOfBirth,
                    Email = user != null ? user.Email : "Unknown",
                    Gender = p.Gender,                   
                };
             
            }).ToList();
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAllAsync();

            var doctorIds = doctors.Select(d => d.UserId).ToList();

            var users = await _userManager.Users.Where(u => doctorIds.Contains(u.Id)).ToListAsync();

            var userDict = users.ToDictionary(u => u.Id);
            return doctors.Select(d =>
            {
                userDict.TryGetValue(d.UserId, out var user);
                return new DoctorDto
                {
                    Id = d.Id,
                    DoctorName = user != null ? user.FirstName + " " + user.LastName : "Unknown",
                    Specialty = d.Specialty,
                    RegistrationNumber = d.RegistrationNumber,
                    ExperienceYears = d.ExperienceYears,
                    Email = user != null ? user.Email : "Unknown",
                    MobileNumber = user != null ? user.PhoneNumber : "Unknown",
                };

            }).ToList();
        }
        public async Task<DoctorDto> GetDoctorByIdAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(p => p.Id == id);
            if (doctor == null)
                throw new KeyNotFoundException("Patient not found.");

            var user = await _userManager.FindByIdAsync(doctor.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return new DoctorDto
            {
                Id = doctor.Id,
                DoctorName = user.FirstName + " " + user.LastName,
                MobileNumber = user.PhoneNumber,
                ExperienceYears = doctor.ExperienceYears,
                RegistrationNumber = doctor.RegistrationNumber,
                Specialty = doctor.Specialty,               
                Email = user.Email,
                Gender = doctor.Gender

            };
        }

        public async Task<IEnumerable<TimeSlotsDto>> GetAllTimeSlotsAsync()
        {
            var timeSlots = await _timeSlotRepository.GetAvailableTimeSlotsAsync();
            return _mapper.Map<List<TimeSlotsDto>>(timeSlots);
        }
        public async Task<IEnumerable<TimeSlotsDto>> GetTimeSlotsByDoctorAsync(int doctorId)
        {
           var timeSlots = await  _timeSlotRepository.GetTimeSlotsByDoctorIdAsync(doctorId);
            return  _mapper.Map<List<TimeSlotsDto>>(timeSlots);
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var totalDoctors = await _doctorRepository.CountAsync();
            var totalPatients = await _patientRepository.CountAsync(p => p.IsActive);

            var totalBookedAppointments = await _appointmentRepository
                .CountAsync(a => a.Status == AppointmentStatus.Booked);

            var totalCancelledAppointments = await _appointmentRepository
                .CountAsync(a => a.Status == AppointmentStatus.Cancelled);

            return new AdminDashboardDto
            {
                TotalDoctors = totalDoctors,
                TotalPatients = totalPatients,
                TotalBookedAppointments = totalBookedAppointments,
                TotalCancelledAppointments = totalCancelledAppointments
            };
        }
        public async Task<TodayDashboardDto> GetTodayDashboardAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var totalDoctors = await _doctorRepository.CountAsync();
            var totalPatients = await _patientRepository.CountAsync(p => p.IsActive);

            var todaysAppointments = await _appointmentRepository
                .GetAppointmentsByDateAsync(today);

            var bookedCount = todaysAppointments.Count(a => a.Status == AppointmentStatus.Booked);
            var completedCount = todaysAppointments.Count(a => a.Status == AppointmentStatus.Completed);
            var cancelledCount = todaysAppointments.Count(a => a.Status == AppointmentStatus.Cancelled);

            var availableDoctors = await _timeSlotRepository
                .CountAsync(ts => ts.SlotDate == today);

            return new TodayDashboardDto
            {                
                BookedAppointments = bookedCount,
                CompletedAppointments = completedCount,
                CancelledAppointments = cancelledCount,
                AvailableDoctors = availableDoctors
            };

        }


    }
}
