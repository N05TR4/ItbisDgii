using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Wrappers;
using MediatR;

namespace ItbisDgii.Application.Features.Contribuyentes.Queries.GetContribuyenteByRncCedula
{
    public class GetContribuyenteByRncCedulaQuery : IRequest<Response<ContribuyenteDto>>
    {
        public string RncCedula { get; set; } = string.Empty;
    }
}
