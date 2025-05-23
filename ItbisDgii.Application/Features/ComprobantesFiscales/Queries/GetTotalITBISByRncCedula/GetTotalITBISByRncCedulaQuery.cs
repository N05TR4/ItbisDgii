using MediatR;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetTotalITBISByRncCedula
{
    public class GetTotalITBISByRncCedulaQuery : IRequest<decimal>
    {
        public string RncCedula { get; set; } = string.Empty;
    }
}
