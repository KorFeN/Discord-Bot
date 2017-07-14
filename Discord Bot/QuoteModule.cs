using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public class QuoteModule : ModuleBase
    {
        public QuoteModule()
        {
        }

        [Command("quote")]
        [Summary("Quote a player")]
        public async Task Quote(string user, [Remainder]string quote)
        {
            if (quote.Length < 5)
            {
                await Context.Channel.SendMessageAsync("quote to short: " + quote);
                return;
            }
            if (Context.Message.MentionedUserIds.Count == 0)
            {
                await Context.Channel.SendMessageAsync("No user mentioned");
                return;
            }
            if (Context.Message.MentionedUserIds.Count >= 2)
            {
                await Context.Channel.SendMessageAsync("Only one user may be mentioned");
                return;
            }

            string line = WebUtility.HtmlEncode(user) + "#" + WebUtility.HtmlEncode(quote);
            
            File.AppendAllText("quotes.txt", line);
            
            await Context.Channel.SendMessageAsync("Quote added: " + quote);
        }
    }
}
