using ItbisDgii.Application.DTOs;
using MediatR;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetAllComprobantesFiscales
{
    public class GetAllComprobantesFiscalesQuery : IRequest<PaginatedResponse<ComprobanteFiscalDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
