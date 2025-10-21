using Parking.Api.Dtos;
using Parking.Api.Models;

namespace Parking.Api.Interfaces.Business
{
    public interface IFaturamentoBusiness
    {
        Task<ImportacaoResponse> ImportarCsv(string base64);
        Task<List<Fatura>> GerarAsync(string competencia, CancellationToken ct = default);
    }
}
