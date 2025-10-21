
using Microsoft.EntityFrameworkCore;

namespace Parking.Api.Dtos
{
    public record GerarFaturaRequest(string Competencia);
    public class FaturaDto
    {
        public Guid Id { get; set; }
        public string Competencia { get; set; }
        public Guid ClienteId { get; set; }
        public decimal Valor { get; set; }
        public DateTime CriadaEm { get; set; }
        public int QtdVeiculos { get; set; }
    }

}
