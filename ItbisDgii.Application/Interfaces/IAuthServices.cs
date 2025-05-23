using ItbisDgii.Application.DTOs.Users;
using ItbisDgii.Application.Wrappers;

namespace ItbisDgii.Application.Interfaces
{
    public interface IAuthServices
    {
        Task<Response<AuthResponseDto>> Authenticate(AuthRequestDto request, string ipAddress);
        Task<Response<string>> RegisterAsync(RegisterRequest request, string origin);
    }
}
