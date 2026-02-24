using AppointmentBooking.Application.DTO.Appointment;
using AppointmentBooking.Application.DTO.Doctor;
using AppointmentBooking.Application.DTO.Patient;
using AppointmentBooking.Application.DTO.TimeSlot;
using AppointmentBooking.Application.Helper;
using AppointmentBooking.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Doctor, CreateDoctorDto>().ReverseMap();
            CreateMap<Doctor, UpdateDoctorDto>().ReverseMap();
            CreateMap<Doctor, DoctorDto>().ReverseMap();

            CreateMap<TimeSlot, CreateTimeSlotDto>().ReverseMap();
            CreateMap<TimeSlot, UpdateTimeSlotDto>().ReverseMap();
            CreateMap<TimeSlot, TimeSlotsDto>()
                .ForMember(x => x.DoctorName, opt => opt.MapFrom(src => src.Doctor.DoctorName))
                .ForMember(x => x.Specialty, opt => opt.MapFrom(src => src.Doctor.Specialty));

            CreateMap<Patient, CreatePatientDto>().ReverseMap();
            CreateMap<Patient, UpdatePatientDto>().ReverseMap();
            CreateMap<Patient, PatientDto>().ReverseMap();
                
            CreateMap<Appointment, CreateAppointmentDto>().ReverseMap();
            CreateMap<Appointment, UpdateAppointmentDto>().ReverseMap();
            CreateMap<Appointment, AppointmentsDto>()
                .ForMember(x=> x.DoctorId, opt => opt.MapFrom(src => src.TimeSlot.DoctorId))
                .ForMember(x => x.DoctorName, opt => opt.MapFrom(src => src.TimeSlot.Doctor.DoctorName))
                .ForMember(x => x.Specialty, opt => opt.MapFrom(src => src.TimeSlot.Doctor.Specialty))
                //.ForMember(x => x.PatientName, opt => opt.MapFrom(src => src.Patient.PatientName))
                .ForMember(x => x.MobileNumber, opt => opt.MapFrom(src => src.Patient.MobileNumber))
                .ForMember(x=> x.Age, opt => opt.MapFrom(src => AgeHelper.CalculateAge(src.Patient.DateOfBirth)))
                .ForMember(x => x.SlotDate, opt => opt.MapFrom(src => src.TimeSlot.SlotDate))
                .ForMember(x => x.StartTime, opt => opt.MapFrom(src => src.TimeSlot.StartTime))
                .ForMember(x => x.EndTime, opt => opt.MapFrom(src => src.TimeSlot.EndTime));
        }
    }
}
