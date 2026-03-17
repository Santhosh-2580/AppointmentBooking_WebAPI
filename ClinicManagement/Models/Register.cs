using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Models
{
    public class Register
    {
        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
