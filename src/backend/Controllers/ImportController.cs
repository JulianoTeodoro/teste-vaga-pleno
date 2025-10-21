
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Interfaces.Business;
using Parking.Api.Models;
using Parking.Api.Services;
using System.Text;

namespace Parking.Api.Controllers
{
    [ApiController]
    [Route("api/import")]
    public class ImportController : ControllerBase
    {
        private readonly IFaturamentoBusiness _faturamentoBusiness;

        public ImportController(IFaturamentoBusiness faturamentoBusiness)
        {
            _faturamentoBusiness = faturamentoBusiness;
        }

        [HttpPost("csv")]
        public async Task<ActionResult<ImportacaoResponse>> ImportCsv([FromBody] ImportarCsv importacao)
        {
            if (string.IsNullOrEmpty(importacao.base64))
                return BadRequest("Arquivo invalido.");

            var importar = await _faturamentoBusiness.ImportarCsv(importacao.base64);

            return Ok(importar);
        }
    }
}
