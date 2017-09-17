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

namespace Discord_Bot.Modules.MOTD
{
    static class MotdModule
    {
        const string jsonfile = "Motds.json";

        static Timer updateTimer;
        static JsonSerializer serializer;
        static Dictionary<ulong, Motd> Motds = new Dictionary<ulong, Motd>(); //TODO: separera Motds mellan olika servrar
        static DiscordSocketClient client;

        public static async Task Start(DiscordSocketClient client)
        {
            MotdModule.client = client;
            serializer = new JsonSerializer();
            serializer.Error += (s, e) => Console.WriteLine("[ERROR][Serializer]" + e);

            await LoadMotds();

            updateTimer = new Timer();
            updateTimer.Enabled = true;
            updateTimer.Elapsed += UpdateChannel;
            updateTimer.Interval = 1000;
            updateTimer.Start();
        }

        internal static void ClearMotd(ulong id)
        {
            lock (Motds)
            {
                Motds.Remove(id);
            }

            SaveMotds();
        }

        private static void UpdateChannel(object sender, ElapsedEventArgs e)
        {
            var currentDate = DateTime.Now.Date;
            if (MotdSettings.Default.LastUpdate.Date != currentDate)
            {
                UpdateMotd().Wait();
                MotdSettings.Default.LastUpdate = currentDate;
                MotdSettings.Default.Save();
            }
        }

        public static async Task UpdateMotd()
        {
            Motd newMotd;
            lock (Motds)
            {
                if (Motds.Count == 0)
                {
                    return;
                }

                int index = new Random().Next(Motds.Count);
                newMotd = Motds.Values.ToArray()[index];
            }

            var MotdUser = client.GetUser(newMotd.CreatorID);

            foreach (var c in client.Guilds
                .Select(p => p.Channels.FirstOrDefault(c => c.Name == "general"))
                .Where(p => p != null)
                .OfType<SocketTextChannel>())
            {
                string newTopic = $"\"{newMotd.MotdText}\" //{MotdUser.Username}";

                await c.ModifyAsync(p => (p as TextChannelProperties).Topic = newTopic)
                    .OnError(ex => Console.WriteLine($"[ERROR][{c.Guild.Name}]Error changing topic: {ex.Message}"));
            }
        }

        static Task LoadMotds()
        {
            if (!File.Exists(jsonfile))
                return Task.CompletedTask;

            return Task.Run(delegate
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(jsonfile)))
                {
                    var jsonReader = new JsonTextReader(reader);
                    lock (Motds)
                    {
                        Motds = serializer.Deserialize<List<Motd>>(jsonReader).ToDictionary(p => p.CreatorID);

                        if (Motds == null)
                        {
                            Console.WriteLine("Error loading Motds");
                            throw new ArgumentException();
                        }
                    }
                }
            });
        }

        static void SaveMotds()
        {
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(jsonfile)))
            {
                lock (Motds)
                {
                    serializer.Serialize(writer, Motds.Values.ToList());
                }
            }
        }

        public static async Task SetMotd(Motd q)
        {
            lock (Motds)
            {
                if (Motds.ContainsKey(q.CreatorID))
                {
                    Motds[q.CreatorID] = q;
                }
                else
                {
                    Motds.Add(q.CreatorID, q);
                }
            }
            
            await Task.Run((Action)SaveMotds);
        }

        public static Motd GetMotdFor(IUser user)
        {
            lock (Motds)
            {
                return Motds.Where(p => p.Key == user.Id).FirstOrDefault().Value;
            }
        }
    }
}
