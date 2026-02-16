using AppointmentBooking.Application.DTO.Appointment;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Enums;
using AppointmentBooking.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IMapper _mapper;
        public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper, ITimeSlotRepository timeSlotRepository)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<AppointmentsDto> CreateAppointmentAsync(CreateAppointmentDto dto)
        {
            var timeSlot = await _timeSlotRepository.GetByIdAsync(x => x.Id == dto.TimeSlotId);

            if (timeSlot == null)
                throw new Exception("Time slot not found");

            if (timeSlot.SlotDate < DateOnly.FromDateTime(DateTime.Today))
                throw new Exception("Cannot book past slot");

            if (timeSlot.BookedCount >= timeSlot.MaxPatients)
                throw new Exception("Slot is full");

            var alreadyBooked = await _appointmentRepository.ExistsAsync(a =>
                a.TimeSlotId == dto.TimeSlotId &&
                a.PatientId == dto.PatientId &&
                a.Status == AppointmentStatus.Booked);

            if (alreadyBooked)
                throw new Exception("Patient already booked this slot");

            var appointment = _mapper.Map<Appointment>(dto);

            appointment.Status = AppointmentStatus.Booked;
            appointment.CreatedOn = DateTime.UtcNow;

            timeSlot.BookedCount++;

            await _appointmentRepository.CreateAsync(appointment);
            await _timeSlotRepository.UpdateAsync(timeSlot);

            return _mapper.Map<AppointmentsDto>(appointment);
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(x => x.Id == id);

            if (appointment == null)
                throw new Exception("Appointment not found");

            if (appointment.Status == AppointmentStatus.Booked)
            {
                var timeSlot = await _timeSlotRepository.GetByIdAsync(x => x.Id == appointment.TimeSlotId);

                if (timeSlot != null && timeSlot.BookedCount > 0)
                {
                    timeSlot.BookedCount--;
                    await _timeSlotRepository.UpdateAsync(timeSlot);
                }
            }

            appointment.Status = AppointmentStatus.Cancelled;
            //appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);
        }

        public async Task<IEnumerable<AppointmentsDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.GetAppointmentDetailsAsync();
            return _mapper.Map<IEnumerable<AppointmentsDto>>(appointments);
        }

        public async Task<AppointmentsDto> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(x => x.Id == id);
            return _mapper.Map<AppointmentsDto>(appointment);
        }

        public async Task<IEnumerable<AppointmentsDto>> GetAppointmentsByFilterAsync(int? doctorId, int? patientId)
        {
            var query = await _appointmentRepository.GetAppointmentDetailsAsync();
            if (doctorId > 0)
            {
                query = query.Where(t => t.TimeSlot.Doctor.Id == doctorId);
            }

            if (patientId > 0)
            {
                query = query.Where(t => t.PatientId == patientId);
            }
            var result = _mapper.Map<List<AppointmentsDto>>(query);

            return result;
        }

        public async Task UpdateAppointmentAsync(int id, UpdateAppointmentDto updateAppointmentDto)
        {
            if (id != updateAppointmentDto.Id)
                throw new ArgumentException("Id mismatch");

            var appointment = await _appointmentRepository.GetByIdAsync(x => x.Id == updateAppointmentDto.Id);

            if (appointment == null)
                throw new Exception("Appointment not found");

            // If status is being changed
            if (appointment.Status != updateAppointmentDto.Status)
            {
                // If cancelling, reduce booked count
                if (updateAppointmentDto.Status == AppointmentStatus.Cancelled &&
                    appointment.Status == AppointmentStatus.Booked)
                {
                    var timeSlot = await _timeSlotRepository.GetByIdAsync(x => x.Id == appointment.TimeSlotId);

                    if (timeSlot != null && timeSlot.BookedCount > 0)
                    {
                        timeSlot.BookedCount--;
                        await _timeSlotRepository.UpdateAsync(timeSlot);
                    }
                }

                appointment.Status = updateAppointmentDto.Status;
            }

            //appointment.UpdatedAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);
        }
    }
}
