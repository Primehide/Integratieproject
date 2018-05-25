using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Account
{
    public class Account
    {
        public int AccountId { get; set; }
        public string IdentityId { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public DateTime GeboorteDatum { get; set; }
        public string Email { get; set; }
        //public string Wachtwoord { get; set; }
        public List<Alert> Alerts { get; set; }
        public Dashboard Dashboard { get; set; }
        public string DeviceId { get; set; }
        public List<Entiteit.Item> Items { get; set; }
        public List<Entiteit.Entiteit> ReviewEntiteiten { get; set; }
        public bool IsAdmin { get; set; } = false;
        public int PlatId { get; set; }
    }
}
