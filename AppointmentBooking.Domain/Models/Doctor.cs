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

        [Required]
        public string DoctorName { get; set; }

        [Required]
        public string Specialty { get; set; }

        public int RegistrationNumber { get; set; }

        [Required]
        public int ExperienceYears { get; set; }

        public string Email { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression("^[6-9][0-9]{9}$", ErrorMessage = "Invalid mobile number")]
        public string MobileNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
