using AppointmentBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.Appointment
{
    public class CreateAppointmentDto
    {
        [Required]
        public int TimeSlotId { get; set; }
        [Required]
        public int PatientId { get; set; }       
    }
}
