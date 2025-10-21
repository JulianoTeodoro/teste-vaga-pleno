using Parking.Api.Dtos;
using Parking.Api.Models;
using System.Linq.Expressions;

namespace Parking.Api.Interfaces.Business
{
    public interface IVeiculosBusiness
    {
        Task<bool> AnyAsync(Expression<Func<Veiculo, bool>>? predicate, CancellationToken ct = default);
        Task<List<Veiculo>> ListAsync(Guid? clienteId);
        Task<Veiculo?> GetById(Guid? id);
        Task<Veiculo> Create(VeiculoCreateDto dto);
        Task<Veiculo> Update(Guid id, VeiculoUpdateDto dto);
        Task Delete(Guid id);
    }
}
