namespace Domain.Entiteit
{
    public class Sleutelwoord
    {
        public Sleutelwoord(string woord) {
            this.Woord = woord;
            }
        public Sleutelwoord()
        {

        }
        public int SleutelwoordId { get; set; }
        public string Woord { get; set; }
    }
}
