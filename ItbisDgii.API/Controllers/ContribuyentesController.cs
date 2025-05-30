﻿using ItbisDgii.Application.DTOs;
using ItbisDgii.Application.Features.Contribuyentes.Commands.CreateContribuyente;
using ItbisDgii.Application.Features.Contribuyentes.Queries.GetAllContribuyentes;
using ItbisDgii.Application.Features.Contribuyentes.Queries.GetContribuyenteByRncCedula;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItbisDgii.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContribuyentesController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ContribuyentesController> _logger;

        public ContribuyentesController(IMediator mediator, ILogger<ContribuyentesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContribuyenteDto>>> GetAll()
        {
            _logger.LogInformation("Getting all contribuyentes");
            var result = await _mediator.Send(new GetAllContribuyentesQuery());
            return Ok(result);
        }


        [HttpGet("{rncCedula}")]
        public async Task<ActionResult<ContribuyenteDto>> GetByRncCedula(string rncCedula)
        {
            _logger.LogInformation("Getting contribuyente by RNC/Cédula: {RncCedula}", rncCedula);
            var result = await _mediator.Send(new GetContribuyenteByRncCedulaQuery { RncCedula = rncCedula });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ContribuyenteDto>> Create(CreateContribuyenteCommand command)
        {
            _logger.LogInformation("Creating new contribuyente with RNC/Cédula: {RncCedula}", command.RncCedula);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByRncCedula), new { rncCedula = result.Data.RncCedula }, result);
        }

    }
}
