using AutoMapper;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetComprobantesFiscalesByRncCedula
{


    public class GetComprobantesFiscalesByRncCedulaQueryHandler : IRequestHandler<GetComprobantesFiscalesByRncCedulaQuery, Response<IEnumerable<ComprobanteFiscalDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetComprobantesFiscalesByRncCedulaQueryHandler> _logger;

        public GetComprobantesFiscalesByRncCedulaQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetComprobantesFiscalesByRncCedulaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<IEnumerable<ComprobanteFiscalDto>>> Handle(GetComprobantesFiscalesByRncCedulaQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting comprobantes fiscales for RNC/Cédula: {RncCedula}", request.RncCedula);

                // Verificar si el contribuyente existe
                var contribuyente = await _unitOfWork.ContribuyenteRepository.GetByRncCedulaAsync(request.RncCedula, cancellationToken);

                if (contribuyente == null)
                {
                    _logger.LogWarning("Contribuyente with RNC/Cédula {RncCedula} not found", request.RncCedula);
                    return new Response<IEnumerable<ComprobanteFiscalDto>>($"Contribuyente con RNC/Cédula {request.RncCedula} no encontrado");
                }

                // Obtener los comprobantes fiscales
                var comprobantes = await _unitOfWork.ComprobanteFiscalRepository.GetByRncCedulaAsync(request.RncCedula, cancellationToken);

                // Convertir a lista y mapear elemento por elemento
                var comprobantesList = comprobantes.ToList();
                var comprobantesDto = comprobantesList.Select(c => _mapper.Map<ComprobanteFiscalDto>(c)).ToList();

                return new Response<IEnumerable<ComprobanteFiscalDto>>(comprobantesDto, "Comprobantes fiscales obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comprobantes fiscales for RNC/Cédula {RncCedula}", request.RncCedula);
                return new Response<IEnumerable<ComprobanteFiscalDto>>($"Error al obtener comprobantes fiscales: {ex.Message}");
            }
        }
    }
}