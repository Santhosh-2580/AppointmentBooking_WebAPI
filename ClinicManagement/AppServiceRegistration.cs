using ClinicManagement.Services;
using ClinicManagement.Services.Appointment;
using ClinicManagement.Services.Interfaces;
using ClinicManagement.Services.Patient;
using ClinicManagement.Services.TimeSlot;

namespace ClinicManagement
{
    public static class AppServiceRegistration
    {
        public static IServiceCollection AddAppServiceRegistraion(this IServiceCollection services)
        {
            services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7220/");
            });

            services.AddHttpClient<AppointmentService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7220/");
            });

            services.AddHttpClient<PatientService>(client => 
            {
                client.BaseAddress = new Uri("https://localhost:7220/");
            });

            services.AddHttpClient<TimeSlotService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7220/");
            });

            return services;

        }
    }
}
