using System;

namespace Discord_Bot.Modules.MOTD
{
    internal class Motd
    {
        public bool Enabled { get; set; }
        public ulong CreatorID { get; set; }
        public DateTime Created { get; set; }
        public string MotdText { get; set; }
    }
}