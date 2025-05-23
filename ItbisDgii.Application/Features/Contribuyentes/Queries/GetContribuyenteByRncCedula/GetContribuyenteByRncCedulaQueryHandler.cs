using AutoMapper;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ItbisDgii.Application.Features.Contribuyentes.Queries.GetContribuyenteByRncCedula
{
    public class GetContribuyenteByRncCedulaQueryHandler : IRequestHandler<GetContribuyenteByRncCedulaQuery, Response<ContribuyenteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetContribuyenteByRncCedulaQueryHandler> _logger;

        public GetContribuyenteByRncCedulaQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetContribuyenteByRncCedulaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ContribuyenteDto>> Handle(GetContribuyenteByRncCedulaQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting contribuyente by RNC/Cédula: {RncCedula}", request.RncCedula);

                var contribuyente = await _unitOfWork.ContribuyenteRepository.GetByRncCedulaAsync(request.RncCedula, cancellationToken);

                if (contribuyente == null)
                {
                    _logger.LogWarning("Contribuyente with RNC/Cédula {RncCedula} not found", request.RncCedula);
                    return new Response<ContribuyenteDto>($"Contribuyente con RNC/Cédula {request.RncCedula} no encontrado");
                }

                var contribuyenteDto = _mapper.Map<ContribuyenteDto>(contribuyente);
                return new Response<ContribuyenteDto>(contribuyenteDto, "Contribuyente obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contribuyente by RNC/Cédula {RncCedula}", request.RncCedula);
                return new Response<ContribuyenteDto>($"Error al obtener contribuyente: {ex.Message}");
            }
        }
    }
}