using Parking.Api.Dtos;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Interfaces.Business
{
    public interface IClienteBusiness
    {
        Task<PagedResults<Cliente>> ListAsync(int pagina, int tamanho, string? filtro, string mensalista, CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<Cliente, bool>>? predicate, CancellationToken ct = default);
        Task<Cliente> CriarCliente(ClienteCreateDto dto);
        Task<Cliente?> GetById(Guid id);
        Task<Cliente> Update(Guid id, ClienteUpdateDto dto);
        Task Delete(Guid id);
    }
}
