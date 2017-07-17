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
        public async Task Quote(IUser user, [Remainder]string quote)
        {
            if (quote.Length < 5)
            {
                await ReplyAsync("quote to short: " + quote);
                return;
            }
            if (Context.Message.MentionedUserIds.Count == 0)
            {
                await ReplyAsync("No user mentioned");
                return;
            }
            if (Context.Message.MentionedUserIds.Count >= 2)
            {
                await ReplyAsync("Only one user may be mentioned");
                return;
            }

            await QuoteModule.AddQuote(new Quote()
            {
                CreatorID =  Context.Message.Author.Id,
                Created = DateTime.Now,
                Enabled = true,
                QuoteText = quote,
                QuotedUserID = user.Id,
                QuoteTime = DateTime.Now,
            });
            
            await ReplyAsync("Quote added: " + quote);
        }
    }
}
