using Parking.Api.Data.Repositories.Base;
using Parking.Api.Interfaces.Repositories;
using Parking.Api.Models;

namespace Parking.Api.Data.Repositories
{
    public class FaturaEFRepository : EFRepositoryBase<Fatura>, IFaturaEFRepository
    {
        public FaturaEFRepository(AppDbContext context) : base(context)
        {
        }
    }
}
