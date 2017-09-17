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

namespace Discord_Bot.Modules.MOTD
{
    public class MotdCommandsModule : ModuleBase
    {
        [Command("motd")]
        [Summary("Set your message of the day")]
        [RequireContext(ContextType.Guild)]
        public async Task Motd([Remainder, Summary("Motd text")]string Motd)
        {
            if (Motd.Length < 5)
            {
                await ReplyAsync("Motd to short: " + Motd);
                return;
            }

            await MotdModule.SetMotd(new Motd()
            {
                CreatorID = Context.Message.Author.Id,
                Created = DateTime.Now,
                Enabled = true,
                MotdText = Motd,
            });

            await ReplyAsync("Motd added: " + Motd);
        }

        [Command("motd")]
        [RequireContext(ContextType.Guild)]
        public async Task Motd()
        {
            MotdModule.ClearMotd(Context.User.Id);

            await ReplyAsync("Motd removed");
        }

        [Command("motdclear")]
        [RequireContext(ContextType.Guild)]
        public async Task MotdClear()
        {
            MotdModule.ClearMotd(Context.User.Id);

            await ReplyAsync("Motd removed");
        }
    }
}
