﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        public List<Faq> Faqs { get; set; }
        public SelectedPeopleVM PeopleChecks { get; set; }
    }


    public class SelectedPeopleVM
    {
        public IEnumerable<SelectListItem> People { get; set; }
    }

}