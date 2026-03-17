using AppointmentBooking.Application.DTO.TimeSlot;
using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Domain.Models;
using AppointmentBooking.Infrastructure.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.ApplicationServices
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;
        public TimeSlotService(ITimeSlotRepository timeSlotRepository, IMapper mapper, UserManager<ApplicationUser> userManager, IDoctorRepository doctorRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _doctorRepository = doctorRepository;
            _timeSlotRepository = timeSlotRepository;            
                     
        }
        public async Task<CreateTimeSlotDto> CreateTimeSlotAsync(CreateTimeSlotDto timeSlotDto, string userId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found for the current user");

            var timeSlotEntity = _mapper.Map<TimeSlot>(timeSlotDto);

            var today = DateOnly.FromDateTime(DateTime.Now);
            var currentTime = DateTime.Now.TimeOfDay;

            if (timeSlotEntity.StartTime < TimeSpan.FromHours(10) || timeSlotEntity.EndTime > TimeSpan.FromHours(21))
                throw new ArgumentException("Time slots must be between 10:00 AM to 9:00 PM");

            if (timeSlotEntity.SlotDate < DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("SlotDate cannot be in the past");

            if (timeSlotEntity.SlotDate == today &&
                timeSlotEntity.StartTime <= currentTime)
                throw new ArgumentException("StartTime cannot be in the past.");

            if ( timeSlotEntity.StartTime >= timeSlotEntity.EndTime)
                throw new ArgumentException("StartTime must be less than EndTime");

            timeSlotEntity.DoctorId = doctor.Id;

            timeSlotEntity.BookedCount = 0; // Initialize BookedCount to 0 for new time slots

            var createdTimeSlot = await _timeSlotRepository.CreateAsync(timeSlotEntity);
            return _mapper.Map<CreateTimeSlotDto>(createdTimeSlot);
        }

        public async Task DeleteTimeSlotAsync(string userId, int id)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found for the current user");

            var timeSlot = await _timeSlotRepository.GetByIdAsync(x => x.Id == id);

            if (timeSlot == null)
                throw new KeyNotFoundException("TimeSlot not found for the current doctor");

            if(timeSlot.DoctorId != doctor.Id)
                throw new UnauthorizedAccessException("You are not allowed to delete this TimeSlot");

            if(timeSlot.BookedCount > 0)
                throw new InvalidOperationException("Cannot delete a time slot that has booked appointments");

            await _timeSlotRepository.DeleteAsync(timeSlot);
        }

        public async Task<TimeSlotsDto> GetTimeSlotByIdAsync(int id)
        {
            var timeSlot = await _timeSlotRepository.GetByIdAsync(x => x.Id == id);
            return _mapper.Map<TimeSlotsDto>(timeSlot);
        }

        public async Task<IEnumerable<TimeSlotsDto>> GetMyTimeSlotsAsync(string userId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found for the current user");

            var timeSlots = await _timeSlotRepository.GetTimeSlotsByDoctorIdAsync(doctor.Id);

            return _mapper.Map<IEnumerable<TimeSlotsDto>>(timeSlots);
        }

        //public async Task<IEnumerable<TimeSlotsDto>> GetTimeSlotsByFilterAsync(int? doctorId)
        //{
        //    var query = await _timeSlotRepository.GetAllTimeSlotsAsync();
        //    if (doctorId > 0)
        //    {
        //        query = query.Where(t => t.DoctorId == doctorId);
        //    }
        //    var result = _mapper.Map<List<TimeSlotsDto>>(query);

        //    return result;
        //}

        public async Task UpdateTimeSlotAsync(string userId, UpdateTimeSlotDto dto, int timeSlotId)
        {
            // 1️. Get doctor using logged-in user
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found for the current user");

            // 2️. Get existing slot
            var existingSlot = await _timeSlotRepository
                .GetByIdAsync(ts => ts.Id == timeSlotId);

            if (existingSlot == null)
                throw new KeyNotFoundException("TimeSlot not found");

            // 3️. Ensure slot belongs to this doctor (VERY IMPORTANT)
            if (existingSlot.DoctorId != doctor.Id)
                throw new UnauthorizedAccessException("You are not allowed to modify this TimeSlot");

            var today = DateOnly.FromDateTime(DateTime.Now);
            var currentTime = DateTime.Now.TimeOfDay;

            // 4️. Validate time range
            if (dto.StartTime.HasValue && dto.EndTime.HasValue)
            {
                if (dto.StartTime < TimeSpan.FromHours(10) || dto.EndTime > TimeSpan.FromHours(21))
                    throw new ArgumentException("Time slots must be between 10:00 AM to 9:00 PM");

                if(dto.SlotDate.HasValue)
                {
                    if (dto.SlotDate < today)
                        throw new ArgumentException("SlotDate cannot be in the past");

                    if (dto.SlotDate == today && dto.StartTime <= currentTime)
                        throw new ArgumentException("StartTime cannot be in the past.");
                }

                if (dto.StartTime >= dto.EndTime)
                    throw new ArgumentException("StartTime must be less than EndTime");
            }

            // 5️. Overlap validation (ONLY for same doctor)
            var isOverlapping = await _timeSlotRepository.ExistsAsync(t =>
                t.DoctorId == doctor.Id &&
                t.SlotDate == dto.SlotDate &&
                t.Id != timeSlotId &&
                dto.StartTime < t.EndTime &&
                dto.EndTime > t.StartTime
            );

            if (isOverlapping)
                throw new InvalidOperationException("Time slot overlaps with existing slot");

            // 6️. Prevent reducing MaxPatients below BookedCount
            if (dto.MaxPatients < existingSlot.BookedCount)
                throw new InvalidOperationException("MaxPatients cannot be less than BookedCount");

            // 7️⃣ Update fields
            if (dto.SlotDate.HasValue)
                existingSlot.SlotDate = dto.SlotDate.Value;

            if (dto.StartTime.HasValue)
                existingSlot.StartTime = dto.StartTime.Value;

            if (dto.EndTime.HasValue)
                existingSlot.EndTime = dto.EndTime.Value;

            if (dto.MaxPatients.HasValue)
            {
                if (dto.MaxPatients.Value < existingSlot.BookedCount)
                    throw new InvalidOperationException("MaxPatients cannot be less than BookedCount");

                existingSlot.MaxPatients = dto.MaxPatients.Value;
            }

            await _timeSlotRepository.UpdateAsync(existingSlot);
        }

        public async Task<IEnumerable<TimeSlotsDto>> GetAllAvailableTimeSlotsAsync()
        {
            var timeSlots = await _timeSlotRepository.GetAvailableTimeSlotsAsync();
            return _mapper.Map<List<TimeSlotsDto>>(timeSlots);
        }
    }
}
