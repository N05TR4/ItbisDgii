using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Wrappers;
using MediatR;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Commands.CreateComprobanteFiscal
{
    public class CreateComprobanteFiscalCommand : IRequest<ComprobanteFiscalDto>
    {
        public string RncCedula { get; set; } 
        public string NCF { get; set; }
        public decimal Monto { get; set; }
    }
}
