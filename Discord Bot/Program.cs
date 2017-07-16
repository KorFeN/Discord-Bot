using System;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Net.Providers.WS4Net;
using System.Linq;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Discord_Bot
{
    public class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            DiscordSocketConfig config = new DiscordSocketConfig();

            config.WebSocketProvider = WS4NetProvider.Instance;
            config.AlwaysDownloadUsers = true;
            config.MessageCacheSize = 1000;
            //TODO: Set Correct DateTimeOffset.
            
            client = new DiscordSocketClient(config);
            client.Log += Log;
            client.MessageReceived += Client_MessageReceived;
            client.MessageUpdated += Client_MessageUpdated;
            client.MessageDeleted += Client_MessageDeleted;

            commands = new CommandService();
            services = new ServiceCollection()
                .BuildServiceProvider();

            await InstallCommands();

            string token = File.ReadAllText(@"..\..\Token.txt");
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        
        public async Task RunCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task Client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (arg2 is SocketTextChannel)
            {
                var channel = arg2 as SocketTextChannel;
                if (channel.Name != "logs")
                {
                    var logChannel = channel.Guild.GetLogChannel();
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
                    var logChannel = channel.Guild.GetLogChannel();

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

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Author.IsBot)
            {
                return;
            }

            await RunCommand(arg);

            Console.WriteLine("<" + arg.Channel + " : " + arg.Author + "> : " + arg.Content);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
