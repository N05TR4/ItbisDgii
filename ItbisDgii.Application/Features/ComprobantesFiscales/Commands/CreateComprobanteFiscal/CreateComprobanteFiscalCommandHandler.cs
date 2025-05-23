using AutoMapper;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Exceptions;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Wrappers;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Commands.CreateComprobanteFiscal
{
    public class CreateComprobanteFiscalCommandHandler : IRequestHandler<CreateComprobanteFiscalCommand, ComprobanteFiscalDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateComprobanteFiscalCommandHandler> _logger;

        public CreateComprobanteFiscalCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateComprobanteFiscalCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ComprobanteFiscalDto> Handle(CreateComprobanteFiscalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar existencia del contribuyente
                var contribuyente = await _unitOfWork.ContribuyenteRepository.GetByRncCedulaAsync(request.RncCedula, cancellationToken);

                if (contribuyente == null)
                {
                    throw new ArgumentException($"Contribuyente con RNC/Cédula {request.RncCedula} no encontrado");
                }

                if (contribuyente.Estatus != EstatusContribuyente.Activo)
                {
                    throw new ApiExceptions($"No se puede crear comprobante para contribuyente inactivo con RNC/Cédula {request.RncCedula}");
                }

                // Crear comprobante
                var comprobante = new ComprobanteFiscal(request.RncCedula, request.NCF, request.Monto);

                await _unitOfWork.ComprobanteFiscalRepository.AddAsync(comprobante, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Comprobante fiscal creado para RNC/Cédula: {RncCedula}", request.RncCedula);

                return _mapper.Map<ComprobanteFiscalDto>(comprobante);
            }
            catch (KeyNotFoundException ex)
            {
                throw ex;
            }
            catch (ApiExceptions ex)
            {
                _logger.LogError(ex, "Error al crear comprobante fiscal para RNC/Cédula {RncCedula}", request.RncCedula);
                throw new ApiExceptions($"Error creando comprobante fiscal: { ex.Message}", ex);
            }
        }

        
        
    }
}
