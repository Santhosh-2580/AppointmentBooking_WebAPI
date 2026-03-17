using ClinicManagement.Models;

namespace ClinicManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<APIResponse<LoginResponse>> Login(Login model);
        Task<APIResponse<object>> Register(Register model);
    }
}
