using System.ComponentModel.DataAnnotations;

namespace Parking.Api.Models
{
    public class ClienteVeiculoVigencia
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Required]
        public Guid VeiculoId { get; set; }
        public Veiculo Veiculo { get; set; }

        [Required]
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }

    }
}
