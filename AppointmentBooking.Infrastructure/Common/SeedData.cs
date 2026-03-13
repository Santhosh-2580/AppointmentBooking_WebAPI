using AppointmentBooking.Application.Common;
using AppointmentBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBooking.Infrastructure.Common
{
    public class SeedData
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Name = "Admin",NormalizedName = "Admin" },
                new IdentityRole { Name = "Patient", NormalizedName = "Patient" },
                new IdentityRole { Name = "Doctor", NormalizedName = "Doctor" }
             };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(role);
                }

            }

            var adminEMail = "adminone@gmail.com";
            var adminPassword = "Admin@1234";

            var existingAdmin = await userManager.FindByEmailAsync(adminEMail);

            if (existingAdmin == null)
            {
                var adminuser = new ApplicationUser
                {
                    UserName = adminEMail,
                    Email = adminEMail,
                    FirstName = "System Admin",
                    LastName = "One",
                    EmailConfirmed = true

                };
                var result = await userManager.CreateAsync(adminuser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminuser, "Admin");
                }
            }          
        }
    }
}
