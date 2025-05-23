using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Features.ComprobantesFiscales.Commands.CreateComprobanteFiscal;
using ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetAllComprobantesFiscales;
using ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetComprobantesFiscalesByRncCedula;
using ItbisDgii.Application.Features.ComprobantesFiscales.Queries.GetTotalITBISByRncCedula;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItbisDgii.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ComprobantesFiscalesController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ComprobantesFiscalesController> _logger;

        public ComprobantesFiscalesController(IMediator mediator, ILogger<ComprobantesFiscalesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComprobanteFiscalDto>>> GetAll()
        {
            _logger.LogInformation("Getting all comprobantes fiscales");
            var result = await _mediator.Send(new GetAllComprobantesFiscalesQuery());
            return Ok(result);
        }

        [HttpGet("contribuyente/{rncCedula}")]
        public async Task<ActionResult<IEnumerable<ComprobanteFiscalDto>>> GetByContribuyente(string rncCedula)
        {
            _logger.LogInformation("Getting comprobantes fiscales for contribuyente: {RncCedula}", rncCedula);
            var result = await _mediator.Send(new GetComprobantesFiscalesByRncCedulaQuery { RncCedula = rncCedula });
            return Ok(result);
        }

        [HttpGet("totales/{rncCedula}")]
        public async Task<ActionResult<decimal>> GetTotalITBIS(string rncCedula)
        {
            _logger.LogInformation("Getting total ITBIS for contribuyente: {RncCedula}", rncCedula);
            var result = await _mediator.Send(new GetTotalITBISByRncCedulaQuery { RncCedula = rncCedula });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ComprobanteFiscalDto>> Create(CreateComprobanteFiscalCommand command)
        {
            _logger.LogInformation("Creating new comprobante fiscal for RNC/Cédula: {RncCedula}", command.RncCedula);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByContribuyente), new { rncCedula = result.RncCedula }, result);
        }

        //[HttpPut("{id}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult> Update(Guid id, UpdateComprobanteFiscalCommand command)
        //{
        //    if (id != command.Id)
        //    {
        //        return BadRequest("ID in URL does not match body");
        //    }

        //    _logger.LogInformation("Updating comprobante fiscal with ID: {Id}", id);
        //    await _mediator.Send(command);
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult> Delete(Guid id)
        //{
        //    _logger.LogInformation("Deleting comprobante fiscal with ID: {Id}", id);
        //    await _mediator.Send(new DeleteComprobanteFiscalCommand { Id = id });
        //    return NoContent();
        //}
    }
}
