using System;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Net.Providers.WS4Net;
using System.Linq;

namespace Discord_Bot
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            DiscordSocketConfig config = new DiscordSocketConfig();

            config.WebSocketProvider = WS4NetProvider.Instance;
            config.AlwaysDownloadUsers = true;
            config.MessageCacheSize = 1000;
            //TODO: Set Correct DateTimeOffset.

            var client = new DiscordSocketClient(config);


            client.Log += Log;
            client.MessageReceived += Client_MessageReceived;
            client.MessageUpdated += Client_MessageUpdated;
            client.MessageDeleted += Client_MessageDeleted;

            string token = File.ReadAllText(@"..\..\Token.txt");
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (arg2 is SocketTextChannel)
            {
                var channel = arg2 as SocketTextChannel;
                if (channel.Name != "logs")
                {
                    var logChannel = channel.Guild.Channels.OfType<SocketTextChannel>().First(p => p.Name == "logs");
                    logChannel.SendMessageAsync("```md" + "\n" +
                        "# Message DELETED" + "\n" +
                        arg1.Value.Timestamp.ToString() + "\n" +
                        "[" + arg1.Value.Channel + "](" + "DELETED by : " + arg1.Value.Author + ")" + "\n" +
                        "\n" +
                        "<MESSAGE >" + "\n" +
                        arg1.Value + "\n" +
                        " ```");

                    Console.WriteLine("<" + arg1.Value.Channel + " : " + arg1.Value.Author + "> [DELETED] : " + arg1.Value + " [BY] " + arg1.Value.Author);
                }
            }

            


            return Task.CompletedTask;
        }

        private Task Client_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            if (arg3 is SocketTextChannel)
            {

                var channel = arg3 as SocketTextChannel;
                if(channel.Name != "logs")
                {
                    var logChannel = channel.Guild.Channels.OfType<SocketTextChannel>().First(p => p.Name == "logs");
                    logChannel.SendMessageAsync("```md" + "\n" +
                        "# Message EDITED" + "\n" +
                        arg2.EditedTimestamp.ToString() + "\n" +
                        "[" + arg2.Channel + "](" + arg2.Author + ")" + "\n" +
                        "\n" +
                        "<FROM >" + "\n" +
                        arg1.Value + "\n" +
                        "<TO >" + "\n" +
                        arg2.Content + "\n" +
                        " ```");
                }
            }

            Console.WriteLine("<" + arg2.Channel + " : " + arg2.Author + "> [EDITED] : " + arg1.Value + " [TO] " + arg2.Content);
            
            return Task.CompletedTask;
        }

        private Task Client_MessageReceived(SocketMessage arg)
        {
            //TODO: Filter out messages from the bot itself

            Console.WriteLine("<" + arg.Channel + " : " + arg.Author + "> : " + arg.Content);
            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
