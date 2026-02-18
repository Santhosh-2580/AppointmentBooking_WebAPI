using AppointmentBooking.Application.DTO.TimeSlot;
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
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IMapper _mapper;

        public TimeSlotService(ITimeSlotRepository timeSlotRepository, IMapper mapper)
        {
            _timeSlotRepository = timeSlotRepository;
            _mapper = mapper;
        }

        public async Task<CreateTimeSlotDto> CreateTimeSlotAsync(CreateTimeSlotDto timeSlotDto)
        {
            var timeSlotEntity = _mapper.Map<TimeSlot>(timeSlotDto);

            timeSlotEntity.BookedCount = 0; // Initialize BookedCount to 0 for new time slots

            var createdTimeSlot = await _timeSlotRepository.CreateAsync(timeSlotEntity);
            return _mapper.Map<CreateTimeSlotDto>(createdTimeSlot);
        }

        public async Task DeleteTimeSlotAsync(int id)
        {
            var timeSlot = await _timeSlotRepository.GetByIdAsync(x => x.Id == id);
            await _timeSlotRepository.DeleteAsync(timeSlot);
        }

        public async Task<IEnumerable<TimeSlotsDto>> GetTimeSlotsAsync()
        {
            var timeSlots = await _timeSlotRepository.GetAllTimeSlotsAsync();
            return _mapper.Map<IEnumerable<TimeSlotsDto>>(timeSlots);
        }

        public async Task<TimeSlotsDto> GetTimeSlotByIdAsync(int id)
        {
            var timeSlot =await _timeSlotRepository.GetByIdAsync(x => x.Id == id);
            return _mapper.Map<TimeSlotsDto>(timeSlot);
        }

        public async Task UpdateTimeSlotAsync(int id, UpdateTimeSlotDto dto)
        {
            if (id != dto.Id)
                throw new ArgumentException("Id mismatch");

            var existingSlot = await _timeSlotRepository.GetByIdAsync(x => x.Id == id);

            if (existingSlot == null)
                throw new KeyNotFoundException("TimeSlot not found");

            // Validate time range
            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be less than EndTime");

            // Overlap validation (exclude same Id)
            var isOverlapping = await _timeSlotRepository.ExistsAsync(t =>
                t.DoctorId == dto.DoctorId &&
                t.SlotDate == dto.SlotDate &&
                t.Id != dto.Id &&
                dto.StartTime < t.EndTime &&
                dto.EndTime > t.StartTime
            );

            if (isOverlapping)
                throw new InvalidOperationException("Time slot overlaps with existing slot");

            // Prevent reducing MaxPatients below BookedCount
            if (dto.MaxPatients < existingSlot.BookedCount)
                throw new InvalidOperationException("MaxPatients cannot be less than BookedCount");

            if (existingSlot.DoctorId != dto.DoctorId && existingSlot.BookedCount > 0)
                throw new InvalidOperationException("Cannot change DoctorId for a slot that already has bookings");

            // Map updated fields
            if (dto.DoctorId != 0) existingSlot.DoctorId = dto.DoctorId;
            if (dto.SlotDate != default) existingSlot.SlotDate = dto.SlotDate;
            if (dto.StartTime != default) existingSlot.StartTime = dto.StartTime;
            if (dto.EndTime != default) existingSlot.EndTime = dto.EndTime;
            if (dto.MaxPatients != 0) existingSlot.MaxPatients = dto.MaxPatients;

            await _timeSlotRepository.UpdateAsync(existingSlot);
        }

        public async Task<IEnumerable<TimeSlotsDto>> GetTimeSlotsByFilterAsync(int? doctorId)
        {
            var query = await _timeSlotRepository.GetAllTimeSlotsAsync();
            if (doctorId > 0)
            {
                query = query.Where(t => t.DoctorId == doctorId);
            }
            var result = _mapper.Map<List<TimeSlotsDto>>(query);

            return result;
        }
    }
}
