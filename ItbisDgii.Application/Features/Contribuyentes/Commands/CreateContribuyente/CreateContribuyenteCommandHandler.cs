using AutoMapper;
using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Wrappers;
using ItbisDgii.Domain.Entities;
using ItbisDgii.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ItbisDgii.Application.Features.Contribuyentes.Commands.CreateContribuyente
{
    public class CreateContribuyenteCommandHandler : IRequestHandler<CreateContribuyenteCommand, Response<ContribuyenteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateContribuyenteCommandHandler> _logger;

        public CreateContribuyenteCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateContribuyenteCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ContribuyenteDto>> Handle(CreateContribuyenteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar si el contribuyente ya existe
                var existingContribuyente = await _unitOfWork.ContribuyenteRepository.GetByRncCedulaAsync(request.RncCedula, cancellationToken);

                if (existingContribuyente != null)
                {
                    return new Response<ContribuyenteDto>($"Contribuyente con RNC/Cédula {request.RncCedula} ya existe");
                }

                // Parse enums
                if (!Enum.TryParse<TipoContribuyente>(request.Tipo, true, out var tipoContribuyente))
                {
                    return new Response<ContribuyenteDto>($"Tipo de contribuyente inválido: {request.Tipo}");
                }

                if (!Enum.TryParse<EstatusContribuyente>(request.Estatus, true, out var estatusContribuyente))
                {
                    return new Response<ContribuyenteDto>($"Estatus de contribuyente inválido: {request.Estatus}");
                }

                // Crear nuevo contribuyente
                var contribuyente = new Contribuyente(
                    request.RncCedula,
                    request.Nombre,
                    tipoContribuyente,
                    estatusContribuyente
                );

                await _unitOfWork.ContribuyenteRepository.AddAsync(contribuyente, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Contribuyente created successfully: {RncCedula}", request.RncCedula);

                var contribuyenteDto = _mapper.Map<ContribuyenteDto>(contribuyente);
                return new Response<ContribuyenteDto>(contribuyenteDto, "Contribuyente creado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contribuyente: {RncCedula}", request.RncCedula);
                return new Response<ContribuyenteDto>($"Error al crear contribuyente: {ex.Message}");
            }
        }
    }
}