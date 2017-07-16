using System;

namespace Discord_Bot.Modules.Quotation
{
    internal class Quote
    {
        public bool Enabled { get; set; }
        public string Creator { get; set; }
        public DateTime Created { get; set; }
        public DateTime QuoteTime { get; set; }
        public string QuoteText { get; set; }
        public string Username { get; set; }
    }
}