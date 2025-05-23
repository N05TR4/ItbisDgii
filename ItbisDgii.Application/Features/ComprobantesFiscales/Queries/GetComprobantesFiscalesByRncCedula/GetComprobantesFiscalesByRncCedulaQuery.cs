using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Wrappers;
using MediatR;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetComprobantesFiscalesByRncCedula
{
    public class GetComprobantesFiscalesByRncCedulaQuery : IRequest<Response<IEnumerable<ComprobanteFiscalDto>>>
    {
        public string RncCedula { get; set; } = string.Empty;
    }
}
