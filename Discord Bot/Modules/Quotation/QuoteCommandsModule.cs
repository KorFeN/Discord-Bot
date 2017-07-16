using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules.Quotation
{
    public class QuoteCommandsModule : ModuleBase
    {
        [Command("quote")]
        [Summary("Quote a player")]
        [RequireContext(ContextType.Guild)]
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

            await QuoteModule.AddQuote(new Quote()
            {
                Creator =  Context.Message.Author.Username,
                Created = DateTime.Now,
                Enabled = true,
                QuoteText = quote,
                Username = user,
                QuoteTime = DateTime.Now,
            });
            
            await Context.Channel.SendMessageAsync("Quote added: " + quote);
        }
    }
}
