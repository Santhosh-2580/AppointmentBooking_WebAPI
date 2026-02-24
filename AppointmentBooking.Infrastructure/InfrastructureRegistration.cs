using AppointmentBooking.Application.Services.Interface;
using AppointmentBooking.Domain.Corntracts;
using AppointmentBooking.Infrastructure.ApplicationServices;
using AppointmentBooking.Infrastructure.Authentication;
using AppointmentBooking.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            // Register Infrastructure services here
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IDoctorRepository , DoctorRepository>();      
            services.AddScoped<ITimeSlotRepository , TimeSlotRepository>();
            services.AddScoped<IPatientRepository , PatientRepository>();
            services.AddScoped<IAppointmentRepository , AppointmentRepository>();

            // Register Application services here
            services.AddScoped<IDoctorService , DoctorService>();
            services.AddScoped<IPatientService , PatientService>();
            services.AddScoped<ITimeSlotService , TimeSlotService>();
            services.AddScoped<IAppointmentService , AppointmentService>();
            return services;
        }
    }
}
