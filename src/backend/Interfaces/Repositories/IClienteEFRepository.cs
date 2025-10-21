using Parking.Api.Dtos;
using Parking.Api.Interfaces.Repositories.Base;
using Parking.Api.Models;

namespace Parking.Api.Interfaces.Repositories
{
    public interface IClienteEFRepository : IRepositoryBase<Cliente>
    {
        Task<PagedResults<Cliente>> ListAsync(int pagina, int tamanho, string? filtro, string mensalista, CancellationToken ct = default);
        Task<Cliente> GetById(Guid id);
    }
}
