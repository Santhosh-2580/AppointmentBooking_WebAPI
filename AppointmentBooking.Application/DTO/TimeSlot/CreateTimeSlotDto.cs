using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.TimeSlot
{
    public class CreateTimeSlotDto
    {
        [Required]
        public int DoctorId { get; set; }    

        public DateOnly SlotDate { get; set; }
        
        [DefaultValue("10:00:00")]
        public TimeSpan StartTime { get; set; }

        [DefaultValue("10:00:00")]
        public TimeSpan EndTime { get; set; }
        public int MaxPatients { get; set; }
    }
}
