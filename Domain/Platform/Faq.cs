﻿namespace Domain.Platform
{
    public class Faq
    {
        public int FaqId { get; set; }
        public string Vraag { get; set; }
        public string Antwoord { get; set; }
        public int PlatformId { get; set; }
    }
}
