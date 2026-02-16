using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.TimeSlot
{
    public class UpdateTimeSlotDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }

        public DateOnly SlotDate { get; set; }

        [DefaultValue("10:00:00")]
        public TimeSpan StartTime { get; set; }

        [DefaultValue("10:00:00")]
        public TimeSpan EndTime { get; set; }

        public int MaxPatients { get; set; }
       
    }
}
