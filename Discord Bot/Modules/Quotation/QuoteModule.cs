using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Discord_Bot.Modules.Quotation
{
    static class QuoteModule
    {
        const string jsonfile = "quotes.json";

        static Timer updateTimer;
        static JsonSerializer serializer;
        static List<Quote> quotes = new List<Quote>();
        static DiscordSocketClient client;

        public static async Task Start(DiscordSocketClient client)
        {
            QuoteModule.client = client;
            serializer = new JsonSerializer();
            serializer.Error += Serializer_Error;

            await LoadQuotes();

            updateTimer = new Timer();
            updateTimer.Enabled = true;
            updateTimer.Elapsed += UpdateChannel;
            updateTimer.Interval = 1000;
            updateTimer.Start();
        }

        private static void Serializer_Error(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            Console.Write(e.ToString());
        }

        private static void UpdateChannel(object sender, ElapsedEventArgs e)
        {
            var currentDate = DateTime.Now.Date;
            if (QuoteSettings.Default.LastUpdate.Date != currentDate)
            {
                UpdateQuote().Wait();
                QuoteSettings.Default.LastUpdate = currentDate;
                QuoteSettings.Default.Save();
            }
        }

        private static async Task UpdateQuote()
        {
            Quote newQuote;
            lock (quotes)
            {
                if (quotes.Count == 0)
                {
                    return;
                }

                int index = new Random().Next(quotes.Count);
                newQuote = quotes[index];
            }

            var quoteUser = client.GetUser(newQuote.QuotedUserID);

            foreach (var c in client.Guilds
                .Select(p => p.Channels.FirstOrDefault(c => c.Name == "general"))
                .Where(p => p != null)
                .OfType<SocketTextChannel>())
            {
                string newTopic = $"\"{newQuote.QuoteText}\" - {quoteUser.Username} {newQuote.QuoteTime:yyyy}";

                await c.ModifyAsync(p => (p as TextChannelProperties).Topic = newTopic)
                    .OnError(ex => Console.WriteLine($"[ERROR][{c.Guild.Name}]Error changing topic: {ex.Message}"));
            }
        }

        static Task LoadQuotes()
        {
            if (!File.Exists(jsonfile))
                return Task.CompletedTask;

            return Task.Run(delegate
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(jsonfile)))
                {
                    var jsonReader = new JsonTextReader(reader);
                    lock (quotes)
                    {
                        quotes = serializer.Deserialize<List<Quote>>(jsonReader);

                        if (quotes == null)
                        {
                            Console.WriteLine("Error loading quotes");
                            throw new ArgumentException();
                        }
                    }
                }
            });
        }

        static void SaveQuotes()
        {
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(jsonfile)))
            {
                lock (quotes)
                {
                    serializer.Serialize(writer, quotes);
                }
            }
        }

        public static async Task AddQuote(Quote q)
        {
            lock (quotes)
            {
                quotes.Add(q);
            }
            
            await Task.Run((Action)SaveQuotes);
        }
    }
}
