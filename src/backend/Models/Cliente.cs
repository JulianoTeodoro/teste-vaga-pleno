
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Parking.Api.Models
{
    public class Cliente
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(200)] public string Nome { get; set; } = string.Empty;
        [MaxLength(20)] public string? Telefone { get; set; }
        [MaxLength(400)] public string? Endereco { get; set; }
        public bool Mensalista { get; set; }
        public decimal? ValorMensalidade { get; set; }
        public DateTime DataInclusao { get; set; } = DateTime.Now;

        [JsonIgnore]
        public List<Veiculo> Veiculos { get; set; } = new();
        [JsonIgnore]
        public List<ClienteVeiculoVigencia> ClienteVeiculoVigencias { get; set; } = new();

    }
}
