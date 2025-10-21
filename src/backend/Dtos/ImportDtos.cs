namespace Parking.Api.Dtos
{
    public record ImportarCsv(string base64);
    public class ImportacaoResponse
    {
        public int QtProcessados { get; set; } = 0;
        public int QtInseridos { get; set; } = 0;
        public int QtErros { get; set; } = 0;
        public List<UnidadeErrorCsv> Erros { get; set; } = new List<UnidadeErrorCsv>();
    }

    public class UnidadeErrorCsv
    {
        public string Erro { get; set; }
        public int? Linha { get; set; }
    }
}
