using ItbisDgii.Application.Exceptions;
using ItbisDgii.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetTotalITBISByRncCedula
{
    public class GetTotalITBISByRncCedulaQueryHandler : IRequestHandler<GetTotalITBISByRncCedulaQuery, decimal>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTotalITBISByRncCedulaQueryHandler> _logger;

        public GetTotalITBISByRncCedulaQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetTotalITBISByRncCedulaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }

        public async Task<decimal> Handle(GetTotalITBISByRncCedulaQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Calculating total ITBIS for RNC/Cédula: {RncCedula}", request.RncCedula);

                var contribuyente = await _unitOfWork.ContribuyenteRepository.GetByRncCedulaAsync(request.RncCedula, cancellationToken);

                if (contribuyente == null)
                {
                    _logger.LogWarning("Contribuyente with RNC/Cédula {RncCedula} not found", request.RncCedula);
                    throw new ArgumentNullException($"Contribuyente con RNC/Cédula {request.RncCedula} no encontrado");
                }

                return contribuyente.CalcularTotalITBIS();
            }
            catch (KeyNotFoundException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total ITBIS for RNC/Cédula {RncCedula}", request.RncCedula);
                throw new ApiExceptions($"Error calculating total ITBIS for RNC/Cédula {request.RncCedula}", ex);
            }
        }
    }
}
