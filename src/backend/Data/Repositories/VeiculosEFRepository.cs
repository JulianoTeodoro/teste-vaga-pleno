using Microsoft.EntityFrameworkCore;
using Parking.Api.Data.Repositories.Base;
using Parking.Api.Dtos;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Models;

namespace Parking.Api.Data.Repositories
{
    public class VeiculosEFRepository : EFRepositoryBase<Veiculo>, IVeiculosEFRepository
    {
        public VeiculosEFRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Veiculo>> ListAsync(Guid? clienteId = null)
        {
            var q = _context.Veiculos.AsQueryable();
            if (clienteId.HasValue) q = q.Where(v => v.ClienteId == clienteId.Value);
            return await q.OrderBy(v => v.Placa).ToListAsync();
        }
    }
}
