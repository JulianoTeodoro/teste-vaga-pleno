using Parking.Api.Interfaces.Repositories.Base;
using Parking.Api.Models;

namespace Parking.Api.Interfaces.Repositories
{
    public interface IVeiculosEFRepository : IRepositoryBase<Veiculo>
    {
        Task<List<Veiculo>> ListAsync(Guid? clienteId = null);
    }
}
