
using Microsoft.EntityFrameworkCore;
using Parking.Api.Data;
using Parking.Api.Models;

namespace Parking.Api.Services
{
    public class FaturamentoService
    {
        private readonly AppDbContext _db;
        public FaturamentoService(AppDbContext db) => _db = db;

        // BUG proposital: usa dono ATUAL do veículo em vez do dono NA DATA DE CORTE
        public async Task<List<Fatura>> GerarAsync(string competencia, CancellationToken ct = default)
        {
            // competencia formato yyyy-MM
            var part = competencia.Split('-');
            var ano = int.Parse(part[0]);
            var mes = int.Parse(part[1]);
            var primeiroDia = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);
            var diasMes = DateTime.DaysInMonth(ano, mes);
            var ultimoDia = new DateTime(ano, mes, diasMes, 23, 59, 59, DateTimeKind.Utc);
            var totalDiasMes = (ultimoDia - primeiroDia).Days + 1;

            var mensalistas = await _db.Clientes
                .Where(c => c.Mensalista)
                .AsNoTracking()
                .ToListAsync(ct);

            var criadas = new List<Fatura>();

            foreach (var cli in mensalistas)
            {
                var existente = await _db.Faturas
                    .FirstOrDefaultAsync(f => f.ClienteId == cli.Id && f.Competencia == competencia, ct);
                if (existente != null) continue; // idempotência simples

                var vigencias = await _db.ClienteVeiculoVigencias
                        .Where(v => v.ClienteId == cli.Id &&
                              (v.DtInicio <= ultimoDia && (v.DtFim == null || v.DtFim >= primeiroDia)))
                        .ToListAsync(ct);


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

                await _db.Faturas.AddAsync(fat);
                criadas.Add(fat);
            }

            await _db.SaveChangesAsync(ct);
            return criadas;
        }
    }
}
