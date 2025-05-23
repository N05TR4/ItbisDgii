using ItbisDgii.Application.DTOs;
using MediatR;

namespace ItbisDgii.Application.Features.Contribuyentes.Queries.GetAllContribuyentes
{
    public class GetAllContribuyentesQuery : IRequest<PaginatedResponse<ContribuyenteDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
