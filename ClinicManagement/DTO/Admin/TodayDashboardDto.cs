using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.DTO.Admin
{
    public class TodayDashboardDto
    {
        public int BookedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int AvailableDoctors { get; set; }
    }
}
