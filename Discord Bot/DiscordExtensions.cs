using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Net.Providers.WS4Net;

namespace Discord_Bot
{
    static class DiscordExtensions
    {
        public static SocketTextChannel GetLogChannel(this SocketGuild guild)
        {
            return guild.Channels.OfType<SocketTextChannel>().First(p => p.Name == "logs");
        }

        public static async Task OnError(this Task task, Action<Exception> onError)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }
    }
}
