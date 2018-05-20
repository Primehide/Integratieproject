using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
   public class Item
    {
        public Item(int EntiteitsID)
        {
          

            EntiteitId = EntiteitsID;
      
        }
        public Item()
        {

        }
        public int ItemId { get; set; }
        public int EntiteitId { get; set; }

    }
}
