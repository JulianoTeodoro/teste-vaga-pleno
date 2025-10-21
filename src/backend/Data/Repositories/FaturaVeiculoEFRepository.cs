using Parking.Api.Data.Repositories.Base;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Models;

namespace Parking.Api.Data.Repositories
{
    public class FaturaVeiculoEFRepository : EFRepositoryBase<FaturaVeiculo>, IFaturaVeiculoEFRepository
    {
        public FaturaVeiculoEFRepository(AppDbContext context) : base(context)
        {
        }
    }
}
