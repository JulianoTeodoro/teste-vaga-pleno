
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Models;
using Parking.Api.Services;

namespace Parking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PlacaService _placa;
        public VeiculosController(AppDbContext db, PlacaService placa) { _db = db; _placa = placa; }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Guid? clienteId = null)
        {
            var q = _db.Veiculos.AsQueryable();
            if (clienteId.HasValue) q = q.Where(v => v.ClienteId == clienteId.Value);
            var list = await q.OrderBy(v => v.Placa).ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeiculoCreateDto dto)
        {
            var placa = _placa.Sanitizar(dto.Placa);
            if (!_placa.EhValida(placa)) return BadRequest("Placa inválida.");
            if (await _db.Veiculos.AnyAsync(v => v.Placa == placa)) return Conflict("Placa já existe.");

            var v = new Veiculo { Placa = placa, Modelo = dto.Modelo, Ano = dto.Ano, ClienteId = dto.ClienteId };

            var vigencia = new ClienteVeiculoVigencia
            {
                ClienteId = dto.ClienteId,
                VeiculoId = v.Id,
                DtInicio = DateTime.Now,
            };

            await _db.Veiculos.AddAsync(v);
            await _db.ClienteVeiculoVigencias.AddAsync(vigencia);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = v.Id }, v);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var v = await _db.Veiculos.FindAsync(id);
            return v == null ? NotFound() : Ok(v);
        }

        // BUG propositado: não invalida/atualiza nada no front; candidato deve ajustar no front (React Query) ou aqui (retornar entidade e orientar)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VeiculoUpdateDto dto)
        {
            var v = await _db.Veiculos.FindAsync(id);
            if (v == null) return NotFound();
            var placa = _placa.Sanitizar(dto.Placa);
            if (!_placa.EhValida(placa)) return BadRequest("Placa inválida.");
            if (await _db.Veiculos.AnyAsync(x => x.Placa == placa && x.Id != id)) return Conflict("Placa já existe.");

            v.Placa = placa;
            v.Modelo = dto.Modelo;
            v.Ano = dto.Ano;

            if (!v.ClienteId.Equals(dto.ClienteId))
            {
                v.ClienteId = dto.ClienteId;
                var vigenciaAtual = await _db.ClienteVeiculoVigencias.FirstOrDefaultAsync(p => p.ClienteId == v.ClienteId && p.VeiculoId == id && p.DtFim == null);

                if(vigenciaAtual != null)
                {
                    vigenciaAtual.DtFim = DateTime.Now;
                }

                await _db.ClienteVeiculoVigencias.AddAsync(new ClienteVeiculoVigencia
                {
                    ClienteId = dto.ClienteId,
                    VeiculoId = id,
                    DtInicio = DateTime.Now,
                });
            }
            await _db.SaveChangesAsync();
            return Ok(v);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var v = await _db.Veiculos.FindAsync(id);
            if (v == null) return NotFound();
            _db.Veiculos.Remove(v);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
