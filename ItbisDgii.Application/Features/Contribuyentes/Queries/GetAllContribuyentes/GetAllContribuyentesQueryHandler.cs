using AutoMapper;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ItbisDgii.Application.Features.Contribuyentes.Queries.GetAllContribuyentes
{
    public class GetAllContribuyentesQueryHandler : IRequestHandler<GetAllContribuyentesQuery, PaginatedResponse<ContribuyenteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllContribuyentesQueryHandler> _logger;

        public GetAllContribuyentesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllContribuyentesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ContribuyenteDto>> Handle(GetAllContribuyentesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting paginated contribuyentes - Page: {PageNumber}, Size: {PageSize}",
                    request.PageNumber, request.PageSize);

                var spec = new ContribuyentesPaginatedSpecification(request.PageNumber, request.PageSize);
                var contribuyentes = await _unitOfWork.ContribuyenteRepository.GetAsync(spec, cancellationToken);
                var totalCount = await _unitOfWork.ContribuyenteRepository.CountAsync(cancellationToken);

                // Convertir IEnumerable a List y luego mapear elemento por elemento
                var contribuyentesList = contribuyentes.ToList();
                var contribuyenteDto = contribuyentesList.Select(c => _mapper.Map<ContribuyenteDto>(c)).ToList();

                return new PaginatedResponse<ContribuyenteDto>
                {
                    Items = contribuyenteDto,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all contribuyentes");
                throw new Exception($"Error retrieving contribuyentes: {ex.Message}", ex);
            }
        }
    }
}