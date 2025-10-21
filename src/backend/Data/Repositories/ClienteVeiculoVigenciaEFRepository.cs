using Parking.Api.Data.Repositories.Base;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Models;

namespace Parking.Api.Data.Repositories
{
    public class ClienteVeiculoVigenciaEFRepository : EFRepositoryBase<ClienteVeiculoVigencia>, IClienteVeiculoVigenciaEFRepository
    {
        public ClienteVeiculoVigenciaEFRepository(AppDbContext context) : base(context)
        {
        }
    }
}
