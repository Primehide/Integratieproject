using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class NotificationViewModel
    {

        public virtual IEnumerable<Domain.Account.Alert> valerts { get; set; }

        
    }
}