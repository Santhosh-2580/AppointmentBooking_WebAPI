using AppointmentBooking.Application.Helper;
using AppointmentBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.DTO.Patient
{
    public class PatientDto
    {
       public int Id { get; set; }
        public string PatientName { get; set; }        
        public string MobileNumber { get; set; } 
        public string Email { get; set; }  
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public int Age => AgeHelper.CalculateAge(DateOfBirth);        

    }
}
