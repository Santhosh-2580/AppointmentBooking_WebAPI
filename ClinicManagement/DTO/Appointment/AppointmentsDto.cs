using ClinicManagement.Enums;

namespace ClinicManagement.DTO.Appointment
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
