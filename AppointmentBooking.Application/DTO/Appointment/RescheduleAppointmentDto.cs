using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.Appointment
{
    public class RescheduleAppointmentDto
    {
        public int AppointmentId { get; set; }
        public int NewTimeSlotId { get; set; }
    }
}
