using AppointmentBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.Patient
{
    public class UpdatePatientDto
    {
        public int Id { get; set; }
        [Required]
        public string PatientName { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression("^[6-9][0-9]{9}$", ErrorMessage = "Invalid mobile number")]
        public string MobileNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
    }
}
