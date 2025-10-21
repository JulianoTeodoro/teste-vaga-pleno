using Parking.Api.Dtos;
using Parking.Api.Interfaces.Repositories.Base;
using Parking.Api.Models;

namespace Parking.Api.Interfaces.Repositories
{
    public interface IFaturaEFRepository : IRepositoryBase<Fatura>
    {
        Task<List<string>> GetPlacas(Guid id);
        Task<List<FaturaDto>> List(string competencia);
    }
}
