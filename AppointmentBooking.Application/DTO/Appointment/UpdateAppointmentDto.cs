using AppointmentBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.Appointment
{
    public class UpdateAppointmentDto
    {
        public int Id { get; set; }
        public int TimeSlotId { get; set; }
        public int PatientId { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
