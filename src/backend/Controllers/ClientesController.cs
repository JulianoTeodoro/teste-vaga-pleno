
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Business;
using Parking.Api.Data;
using Parking.Api.Data.Repositories;
using Parking.Api.Dtos;
using Parking.Api.Interfaces.Business;
using Parking.Api.Models;

namespace Parking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteBusiness _clienteBusiness;
        private readonly IVeiculosBusiness _veiculosBusiness;

        public ClientesController(IClienteBusiness clienteBusiness, IVeiculosBusiness veiculosBusiness)
        {
            _clienteBusiness = clienteBusiness;
            _veiculosBusiness = veiculosBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamanho = 10, [FromQuery] string? filtro = null, [FromQuery] string mensalista = "all")
        {
            var clientes = await _clienteBusiness.ListAsync(pagina, tamanho, filtro, mensalista);

            return Ok(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteCreateDto dto)
        {
            var existe = await _clienteBusiness.AnyAsync(c => c.Nome == dto.Nome && c.Telefone == dto.Telefone);
            if (existe) return Conflict("Cliente já existe.");

            var cliente = await _clienteBusiness.CriarCliente(dto);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var c = await _clienteBusiness.GetById(id);
            return c == null ? NotFound("Cliente Não Existente. ") : Ok(c);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ClienteUpdateDto dto)
        {
            var c = await _clienteBusiness.GetById(id);
            if (c == null) return NotFound("Cliente não cadastrado!!");

            var existe = await _clienteBusiness.AnyAsync(c => c.Id != id && c.Nome == dto.Nome && c.Telefone == dto.Telefone);
            if (existe) return Conflict("Cliente já existente .");

            var cliente = await _clienteBusiness.Update(id, dto);

            return Ok(cliente);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var c = await _clienteBusiness.GetById(id);
            if (c == null) return NotFound("Cliente Não Existente.");
            var temVeiculos = await _veiculosBusiness.AnyAsync(v => v.ClienteId == id);
            if (temVeiculos) return BadRequest("Cliente possui veículos associados. Transfira ou remova antes.");
            await _clienteBusiness.Delete(id);
            return NoContent();
        }
    }
}
