namespace Domain.Entiteit
{
   public class Item
    {
        public Item(int entiteitsId)
        {
            EntiteitId = entiteitsId;
        }
        public Item()
        {

        }
        public int ItemId { get; set; }
        public int EntiteitId { get; set; }

    }
}
