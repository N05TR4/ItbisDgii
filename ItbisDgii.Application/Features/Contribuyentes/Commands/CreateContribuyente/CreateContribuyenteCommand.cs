using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Wrappers;
using MediatR;

namespace ItbisDgii.Application.Features.Contribuyentes.Commands.CreateContribuyente
{
    public class CreateContribuyenteCommand : IRequest<Response<ContribuyenteDto>>
    {
        public string RncCedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Estatus { get; set; } = "Activo";
    }
}
