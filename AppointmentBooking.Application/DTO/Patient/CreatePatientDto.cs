using AppointmentBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.Patient
{
    public class CreatePatientDto
    {
       
        [Required]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression("^[6-9][0-9]{9}$", ErrorMessage = "Invalid mobile number")]
        public string MobileNumber { get; set; }       

        [Required]
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
    }
}
