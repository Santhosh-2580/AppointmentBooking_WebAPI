using AppointmentBooking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Domain.Models
{
    public class TimeSlot : BaseModel
    {
        // 👇 FK
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public DateOnly SlotDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int MaxPatients { get; set; } = 3;
        public int BookedCount { get; set; } = 0;

        

        //public Appointment Appointment { get; set; }

    }
}
