using AppointmentBooking.Domain.Common;
using AppointmentBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Domain.Models
{
    public class Appointment : BaseModel
    {               
        public int TimeSlotId { get; set; } // FK
        public TimeSlot TimeSlot { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }       
        public  AppointmentStatus Status { get; set; }

    }
}
