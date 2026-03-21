using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.DTO.Admin
{
    public class AdminDashboardDto
    {
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalBookedAppointments { get; set; }
        public int TotalCancelledAppointments { get; set; }
    }
}
