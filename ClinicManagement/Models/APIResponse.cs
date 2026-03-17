namespace ClinicManagement.Models
{
    public class APIResponse<T>
    {
        public string StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public T Result { get; set; }
        public string DisplayMessage { get; set; }
    }
}
