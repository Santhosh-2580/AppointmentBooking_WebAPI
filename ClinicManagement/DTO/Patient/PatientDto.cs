using ClinicManagement.Enums;
using ClinicManagement.Helper;

namespace ClinicManagement.DTO.Patient
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
