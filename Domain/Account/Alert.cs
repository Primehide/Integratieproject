namespace Domain.Account
{
    public class Alert
    {
        public int AlertId { get; set; }
        public string AlertNaam { get; set; }
        public Domain.Enum.TrendType TrendType { get; set; }
        public Domain.Enum.Voorwaarde Voorwaarde { get; set; }
        public double MinWaarde { get; set; }
        public Domain.Enum.PlatformType PlatformType { get; set; }
        public Account Account { get; set; }
        public Domain.Entiteit.Entiteit Entiteit { get; set; }
     
        public bool Triggered { get; set; }
    }
}
