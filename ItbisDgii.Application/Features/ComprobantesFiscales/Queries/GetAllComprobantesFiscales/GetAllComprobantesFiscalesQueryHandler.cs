using AutoMapper;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Exceptions;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetAllComprobantesFiscales
{
    public class GetAllComprobantesFiscalesQueryHandler : IRequestHandler<GetAllComprobantesFiscalesQuery, PaginatedResponse<ComprobanteFiscalDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllComprobantesFiscalesQueryHandler> _logger;

        public GetAllComprobantesFiscalesQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAllComprobantesFiscalesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ComprobanteFiscalDto>> Handle(GetAllComprobantesFiscalesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting all comprobantes fiscales paginated - Page: {PageNumber}, Size: {PageSize}",
                    request.PageNumber, request.PageSize);

                var spec = new ComprobanteFiscalPaginatedSpecification(request.PageNumber, request.PageSize);

                var comprobantes = await _unitOfWork.ComprobanteFiscalRepository.GetAsync(spec, cancellationToken);
                var totalCount = await _unitOfWork.ComprobanteFiscalRepository.CountAsync(cancellationToken);

                // Convertir IEnumerable a List y luego mapear
                var comprobantesList = comprobantes.ToList();
                var comprobanteFiscalDto = comprobantesList.Select(c => _mapper.Map<ComprobanteFiscalDto>(c)).ToList();

                _logger.LogInformation("Found {Count} comprobantes fiscales", comprobantesList.Count);

                return new PaginatedResponse<ComprobanteFiscalDto>
                {
                    Items = comprobanteFiscalDto,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all comprobantes fiscales");
                throw new ApiExceptions("Error retrieving comprobantes fiscales", ex);
            }
        }
    }
}