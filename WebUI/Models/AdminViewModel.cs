using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Post;
using Domain.Entiteit;
using Domain.Account;

namespace WebUI.Models
{
    public class AdminViewModel
    {
        public List<Post> RecentePosts { get; set; }
        public List<Entiteit> AlleEntiteiten { get; set; }
        public List<Account> Users { get; set; }
    }
}