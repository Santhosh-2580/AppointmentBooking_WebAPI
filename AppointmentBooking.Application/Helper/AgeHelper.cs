using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application.Helper
{
    public static class AgeHelper
    {
        public static int CalculateAge(DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            int age = today.Year - dob.Year;

            if (dob > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
