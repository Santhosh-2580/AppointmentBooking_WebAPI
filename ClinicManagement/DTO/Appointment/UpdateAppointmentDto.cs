using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicManagement.Enums;

namespace ClinicManagement.DTO.Appointment
{
    public class UpdateAppointmentDto
    {
        public int Id { get; set; }           
        public AppointmentStatus Status { get; set; }
    }
}
