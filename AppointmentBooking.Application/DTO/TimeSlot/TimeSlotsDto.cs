using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.TimeSlot
{
    public class TimeSlotsDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }

        public string DoctorName { get; set; }
        public string Specialty { get; set; }

        public DateOnly SlotDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int MaxPatients { get; set; }
        public int BookedCount { get; set; }
    }
}
