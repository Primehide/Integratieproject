using System.Collections.Generic;

namespace WebUI.Models
{
    public class PlatformAdminModel
    {
        public Domain.Platform.Deelplatform Deelplatform { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}