namespace Parking.Api.Dtos
{
    public class PagedResults<T>
    {
        public List<T> Itens { get; set; } = new List<T>();
        public int Total { get; set; }
    }
}
