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
        public async Task Quote(
            [Summary("Mention a User")]IUser user,
            [Remainder, Summary("Quote text")]string quote)
        {
            if (quote.Length < 5)
            {
                await ReplyAsync("quote to short: " + quote);
                return;
            }

            await QuoteModule.AddQuote(new Quote()
            {
                CreatorID = Context.Message.Author.Id,
                Created = DateTime.Now,
                Enabled = true,
                QuoteText = quote,
                QuotedUserID = user.Id,
                QuoteTime = DateTime.Now,
            });

            await ReplyAsync("Quote added: " + quote);
        }

        [Command("quotefrom")]
        [Alias("qfrom")]
        [Summary("Get quotes made by user")]
        [RequireContext(ContextType.Guild)]
        public async Task QuoteFrom(IUser user = null)
        {
            var userQuotes = QuoteModule.GetQuotesFrom(user);
            var result = string.Join("\n", userQuotes.Select((q, index) => $"#{index} \"{q.QuoteText}\" {q.QuoteTime:yyyy-MM-dd}"));

            await ReplyAsync(result)
                .OnError(ex => Console.WriteLine("[ERROR]" + ex.Message));
        }
    }
}
