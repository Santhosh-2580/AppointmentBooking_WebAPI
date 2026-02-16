using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.Doctor
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public int RegistrationNumber { get; set; }
        public int ExperienceYears { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
    }
}
