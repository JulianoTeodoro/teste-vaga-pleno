
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Business;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Interfaces.Business;
using Parking.Api.Models;
using Parking.Api.Services;

namespace Parking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        private readonly IVeiculosBusiness _veiculosBusiness;
        private readonly PlacaService _placa;
        public VeiculosController(PlacaService placa, IVeiculosBusiness veiculosBusiness) { _placa = placa; _veiculosBusiness = veiculosBusiness; }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Guid? clienteId = null)
        {
            var list = await _veiculosBusiness.ListAsync(clienteId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeiculoCreateDto dto)
        {
            var placa = _placa.Sanitizar(dto.Placa);
            if (!_placa.EhValida(placa)) return BadRequest("Placa inválida.");
            if (await _veiculosBusiness.AnyAsync(v => v.Placa == placa)) return Conflict("Placa já existe.");

            var v = await _veiculosBusiness.Create(dto);

            return CreatedAtAction(nameof(GetById), new { id = v.Id }, v);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var v = await _veiculosBusiness.GetById(id);
            return v == null ? NotFound("Veiculo Não Existente.") : Ok(v);
        }

        // BUG propositado: não invalida/atualiza nada no front; candidato deve ajustar no front (React Query) ou aqui (retornar entidade e orientar)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VeiculoUpdateDto dto)
        {
            var v = await _veiculosBusiness.GetById(id);
            if (v == null) return NotFound("Veiculo Não Existente.");
            var placa = _placa.Sanitizar(dto.Placa);
            if (!_placa.EhValida(placa)) return BadRequest("Placa inválida.");
            if (await _veiculosBusiness.AnyAsync(x => x.Placa == placa && x.Id != id)) return Conflict("Placa já existe.");

            var vehicle = await _veiculosBusiness.Update(id, dto);

            return Ok(vehicle);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var v = await _veiculosBusiness.GetById(id);
            if (v == null) return NotFound("Veiculo Não Existente.");
            await _veiculosBusiness.Delete(id);
            return NoContent();
        }
    }
}
