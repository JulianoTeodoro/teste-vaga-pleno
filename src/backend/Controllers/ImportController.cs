
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Dtos;
using Parking.Api.Models;
using Parking.Api.Services;
using System.Text;

namespace Parking.Api.Controllers
{
    [ApiController]
    [Route("api/import")]
    public class ImportController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PlacaService _placa;
        public ImportController(AppDbContext db, PlacaService placa) { _db = db; _placa = placa; }

        [HttpPost("csv")]
        public async Task<ActionResult<ImportacaoResponse>> ImportCsv([FromBody] ImportarCsv importacao)
        {
            if (string.IsNullOrEmpty(importacao.base64))
                return BadRequest("Arquivo invalido.");

            var importacaoResponse = new ImportacaoResponse();

            byte[] bytes = Convert.FromBase64String(importacao.base64);

            using var s = new MemoryStream(bytes);
            using var r = new StreamReader(s, Encoding.UTF8);

            var linha = 0;

            string? header = await r.ReadLineAsync(); 
            while (!r.EndOfStream)
            {
                linha++;
                var raw = await r.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(raw)) continue;
                importacaoResponse.QtProcessados++;

                // CSV simples separado por vírgula: placa,modelo,ano,cliente_identificador,cliente_nome,cliente_telefone,cliente_endereco,mensalista,valor_mensalidade
                var cols = raw.Split(',');
                try
                {
                    var placa = _placa.Sanitizar(cols[0]);
                    var modelo = cols[1];
                    int? ano = int.TryParse(cols[2], out var _ano) ? _ano : null;
                    var cliId = cols[3];
                    var cliNome = cols[4];
                    var cliTel = new string((cols[5] ?? "").Where(char.IsDigit).ToArray());
                    var cliEnd = cols[6];
                    bool mensalista = bool.TryParse(cols[7], out var m) && m;
                    decimal? valorMens = decimal.TryParse(cols[8], out var vm) ? vm : null;

                    if (!_placa.EhValida(placa)) throw new Exception("Placa inválida para importação.");
                    if (await _db.Veiculos.AnyAsync(v => v.Placa == placa)) throw new Exception("Placa duplicada.");

                    var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Nome == cliNome && c.Telefone == cliTel);
                    if (cliente == null)
                    {
                        cliente = new Cliente { Nome = cliNome, Telefone = cliTel, Endereco = cliEnd, Mensalista = mensalista, ValorMensalidade = valorMens };
                        _db.Clientes.Add(cliente);
                        await _db.SaveChangesAsync();
                    }

                    var v = new Veiculo { Placa = placa, Modelo = modelo, Ano = ano, ClienteId = cliente.Id };
                    _db.Veiculos.Add(v);
                    await _db.SaveChangesAsync();
                    importacaoResponse.QtInseridos++;
                }
                catch (Exception ex)
                {
                    importacaoResponse.Erros.Add(
                        new UnidadeErrorCsv
                        {
                            Erro = $"{ex.Message} (Conteudo = '{raw}')",
                            Linha = linha
                        }
                    );
                    importacaoResponse.QtErros++;
                }
            }

            return Ok(importacaoResponse);
        }
    }
}
