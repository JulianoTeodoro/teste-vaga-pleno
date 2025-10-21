using Microsoft.EntityFrameworkCore;
using Parking.Api.Data.Repositories.Base;
using Parking.Api.Dtos;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Models;

namespace Parking.Api.Data.Repositories
{
    public class ClienteEFRepository : EFRepositoryBase<Cliente>, IClienteEFRepository
    {
        public ClienteEFRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResults<Cliente>> ListAsync(int pagina, int tamanho, string? filtro, string mensalista, CancellationToken ct = default)
        {
            var q = _context.Clientes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtro))
                q = q.Where(c => c.Nome.Contains(filtro));
            if (mensalista == "true") q = q.Where(c => c.Mensalista);
            if (mensalista == "false") q = q.Where(c => !c.Mensalista);
            var total = await q.CountAsync(ct);
            var itens = await q
                .OrderBy(c => c.Nome)
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync(ct);

            return new PagedResults<Cliente>
            {
                Itens = itens,
                Total = total
            };
        }

        public async Task<Cliente> GetById(Guid id)
        {
            var c = await _context.Clientes.Include(x => x.Veiculos).FirstOrDefaultAsync(x => x.Id == id);

            return c;
        }
    }
}
