using ItbisDgii.Application.DTOs.Users;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Wrappers;
using MediatR;

namespace ItbisDgii.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response<string>>
    {
        private readonly IAuthServices _authServices;

        public RegisterCommandHandler(IAuthServices authServices)
        {
            _authServices = authServices ?? throw new ArgumentNullException(nameof(authServices));
        }

        public async Task<Response<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _authServices.RegisterAsync(new RegisterRequest
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword
            }, request.Origin);
        }
    }
}
