using Microsoft.EntityFrameworkCore;
using Parking.Api.Data.Repositories.Base;
using Parking.Api.Dtos;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Models;

namespace Parking.Api.Data.Repositories
{
    public class FaturaEFRepository : EFRepositoryBase<Fatura>, IFaturaEFRepository
    {
        public FaturaEFRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<string>> GetPlacas(Guid id)
        {
            var placas = await _context.FaturasVeiculos
                .Where(x => x.FaturaId == id)
                .Join(_context.Veiculos, fv => fv.VeiculoId, v => v.Id, (fv, v) => v.Placa)
                .ToListAsync();

            return placas;
        }

        public async Task<List<FaturaDto>> List(string competencia)
        {
            var q = _context.Faturas.AsQueryable();
            if (!string.IsNullOrWhiteSpace(competencia)) q = q.Where(f => f.Competencia == competencia);
            var list = await q
                .OrderByDescending(f => f.CriadaEm)
                .Select(f => new FaturaDto
                {
                    Id = f.Id, Competencia = f.Competencia, ClienteId = f.ClienteId, Valor = f.Valor, CriadaEm = f.CriadaEm, QtdVeiculos = _context.FaturasVeiculos.Count(x => x.FaturaId == f.Id)
                })
                .ToListAsync();

            return list;
        }
    }
}
