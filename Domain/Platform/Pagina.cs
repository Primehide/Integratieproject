namespace Domain.Platform
{
    public class Pagina
    {
        public int PaginaId { get; set; }
        public string Afbeelding { get; set; }
        public Domain.Entiteit.Entiteit Entiteit { get; set; }
    }
}
