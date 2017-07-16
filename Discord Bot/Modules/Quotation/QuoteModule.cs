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

        static void Start()
        {
            serializer = new JsonSerializer();
            LoadQuotes();

            updateTimer.Enabled = true;
            updateTimer.Elapsed += UpdateChannel;
            updateTimer.Interval = 100;
            updateTimer.Start();
        }

        private static void UpdateChannel(object sender, ElapsedEventArgs e)
        {
            var currentDate = DateTime.Now.Date;
            if (QuoteSettings.Default.LastUpdate.Date != currentDate)
            {
                UpdateQuote();
                QuoteSettings.Default.LastUpdate = currentDate;
                QuoteSettings.Default.Save();
            }
        }

        private static void UpdateQuote()
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        static void LoadQuotes()
        {
            if (!File.Exists(jsonfile))
                return;

            using (StreamReader reader = new StreamReader(File.OpenRead(jsonfile)))
            {
                var jsonReader = new JsonTextReader(reader);
                quotes = serializer.Deserialize<List<Quote>>(jsonReader);
            }
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
            quotes.Add(q);

            await Task.Run((Action)SaveQuotes);
        }
    }
}
