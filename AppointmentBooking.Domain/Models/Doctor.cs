using AppointmentBooking.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Domain.Models
{
    public class Doctor : BaseModel
    {
        public string UserId { get; set; }

        [Required]
        public string DoctorName { get; set; }

        [Required]
        public string Specialty { get; set; }

        public int RegistrationNumber { get; set; }

        [Required]
        public int ExperienceYears { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
