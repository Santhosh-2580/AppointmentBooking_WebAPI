using AppointmentBooking.Application.Common;
using AppointmentBooking.Application.Services;
using AppointmentBooking.Application.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Application
{
    public static class ApplicationRegistration
    {
       public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<ITimeSlotService, TimeSlotService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            return services;
        }
    }
}
