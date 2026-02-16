using AppointmentBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.Appointment
{
    public class AppointmentsDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public Gender Gender { get; set; }
        public int Age { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }

        public int TimeSlotId { get; set; }
        public DateOnly SlotDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
