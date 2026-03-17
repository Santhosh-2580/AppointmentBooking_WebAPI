using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.DTO.Appointment
{
    public class CreateAppointmentDto
    {
        [Required]
        public int TimeSlotId { get; set; }             
    }
}
