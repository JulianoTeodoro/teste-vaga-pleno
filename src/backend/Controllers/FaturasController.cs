
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Interfaces.Business;
using Parking.Api.Models;
using Parking.Api.Services;

namespace Parking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaturasController : ControllerBase
    {
        private readonly IFaturamentoBusiness _fat;
        public FaturasController(IFaturamentoBusiness fat) { _fat = fat; }

        [HttpPost("gerar")]
        public async Task<IActionResult> Gerar([FromBody] GerarFaturaRequest req, CancellationToken ct)
        {
            var criadas = await _fat.GerarAsync(req.Competencia, ct);
            return Ok(new { criadas = criadas.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? competencia = null)
        {
            var lista = await _fat.ListAsync(competencia);
            return Ok(lista);
        }

        [HttpGet("{id:guid}/placas")]
        public async Task<IActionResult> Placas(Guid id)
        {
            var placas = await _fat.GetPlacas(id);
            return Ok(placas);
        }
    }
}
