using Parking.Api.Dtos;
using Parking.Api.Interfaces.Business;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Models;
using Parking.Api.Services;
using System.Text;

namespace Parking.Api.Business
{
    public class FaturamentoBusiness : IFaturamentoBusiness
    {
        private readonly PlacaService _placaService;
        private readonly IClienteEFRepository _clienteEFRepository;
        private readonly IVeiculosEFRepository _veiculosEFRepository;
        private readonly IFaturaEFRepository _faturaEFRepository;
        private readonly IClienteVeiculoVigenciaEFRepository _clienteVeiculoVigenciaEFRepository;

        public FaturamentoBusiness(PlacaService placaService, IClienteEFRepository clienteEFRepository, IVeiculosEFRepository veiculosEFRepository, 
            IFaturaEFRepository faturaEFRepository, IClienteVeiculoVigenciaEFRepository clienteVeiculoVigenciaEFRepository)
        {
            _placaService = placaService;
            _clienteEFRepository = clienteEFRepository;
            _veiculosEFRepository = veiculosEFRepository;
            _faturaEFRepository = faturaEFRepository;
            _clienteVeiculoVigenciaEFRepository = clienteVeiculoVigenciaEFRepository;
        }

        public async Task<List<Fatura>> GerarAsync(string competencia, CancellationToken ct = default)
        {
            // competencia formato yyyy-MM
            var part = competencia.Split('-');
            var ano = int.Parse(part[0]);
            var mes = int.Parse(part[1]);
            var primeiroDia = new DateTime(ano, mes, 1, 0, 0, 0);
            var diasMes = DateTime.DaysInMonth(ano, mes);
            var ultimoDia = new DateTime(ano, mes, diasMes, 23, 59, 59);
            var totalDiasMes = (ultimoDia - primeiroDia).Days + 1;

            var mensalistas = await _clienteEFRepository.GetListAsync(p => p.Mensalista);

            var criadas = new List<Fatura>();

            foreach (var cli in mensalistas)
            {
                var existente = await _faturaEFRepository
                    .GetFirstOrDefaultAsync(f => f.ClienteId == cli.Id && f.Competencia == competencia);
                if (existente != null) continue; // idempotência simples

                var vigencias = await _clienteVeiculoVigenciaEFRepository
                        .GetListAsync(v => v.ClienteId == cli.Id &&
                              (v.DtInicio <= ultimoDia && (v.DtFim == null || v.DtFim >= primeiroDia)));


                if (!vigencias.Any())
                    continue;

                var valorMensal = cli.ValorMensalidade ?? 0m;
                var valorProporcional = 0M;

                foreach (var vigencia in vigencias)
                {
                    var inicioReal = vigencia.DtInicio > primeiroDia ? vigencia.DtInicio : primeiroDia;
                    var fimReal = !vigencia.DtFim.HasValue || vigencia.DtFim.Value > ultimoDia ? ultimoDia : vigencia.DtFim.Value;

                    if (inicioReal > ultimoDia || fimReal < primeiroDia)
                        continue;

                    var diasAtivos = (fimReal - inicioReal).Days + 1;
                    if (diasAtivos > 0)
                    {
                        valorProporcional += Math.Round(valorMensal * diasAtivos / totalDiasMes, 2);
                        continue;
                    }
                }

                var fat = new Fatura
                {
                    Competencia = competencia,
                    ClienteId = cli.Id,
                    Valor = valorProporcional,
                    Observacao = $"Fatura gerada com sucesso. Dias faturados: {totalDiasMes} "
                };

                foreach (var vig in vigencias)
                    fat.Veiculos.Add(new FaturaVeiculo { FaturaId = fat.Id, VeiculoId = vig.VeiculoId });

                await _faturaEFRepository.CreateAsync(fat);
                criadas.Add(fat);
            }
            await _faturaEFRepository.SaveChangesAsync();

            return criadas;

        }

        public async Task<ImportacaoResponse> ImportarCsv(string base64)
        {
            var importacaoResponse = new ImportacaoResponse();

            byte[] bytes = Convert.FromBase64String(base64);

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
                    var placa = _placaService.Sanitizar(cols[0]);
                    var modelo = cols[1];
                    int? ano = int.TryParse(cols[2], out var _ano) ? _ano : null;
                    var cliId = cols[3];
                    var cliNome = cols[4];
                    var cliTel = new string((cols[5] ?? "").Where(char.IsDigit).ToArray());
                    var cliEnd = cols[6];
                    bool mensalista = bool.TryParse(cols[7], out var m) && m;
                    decimal? valorMens = decimal.TryParse(cols[8], out var vm) ? vm : null;

                    if (!_placaService.EhValida(placa)) throw new Exception("Placa inválida para importação.");
                    if (await _veiculosEFRepository.AnyAsync(v => v.Placa == placa)) throw new Exception("Placa duplicada.");

                    var cliente = await _clienteEFRepository.GetFirstOrDefaultAsync(c => c.Nome == cliNome && c.Telefone == cliTel);
                    if (cliente == null)
                    {
                        cliente = new Cliente { Nome = cliNome, Telefone = cliTel, Endereco = cliEnd, Mensalista = mensalista, ValorMensalidade = valorMens };
                        await _clienteEFRepository.CreateAsync(cliente);
                        await _clienteEFRepository.SaveChangesAsync();
                    }

                    var v = new Veiculo { Placa = placa, Modelo = modelo, Ano = ano, ClienteId = cliente.Id };
                    await _veiculosEFRepository.CreateAsync(v);
                    await _veiculosEFRepository.SaveChangesAsync();
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

            return importacaoResponse;
        }
    }
}
