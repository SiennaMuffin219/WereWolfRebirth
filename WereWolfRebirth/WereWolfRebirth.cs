using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Exceptions;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;

namespace WereWolfRebirth
{
    class Program
    {
        private DiscordClient client;
        private CommandsNextModule commands;

        public static List<Personnage> users;


        static void Main(string[] args) => new Program().AsyncMain().GetAwaiter().GetResult();

        public async Task AsyncMain()
        {
            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

            client = new DiscordClient(new DiscordConfiguration()
            {
                LogLevel = LogLevel.Debug,
                Token = config.token,
            });

            client.DebugLogger.LogMessageReceived += DebugLogger_LogMessageReceived;
            commands = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                StringPrefix = config.prefix
            });
            commands.RegisterCommands<BotCommands>();

            client.UseInteractivity(new InteractivityConfiguration());

            //client.MessageCreated += Client_MessageCreated;

            try
            {
                await client.ConnectAsync();
            }
            catch (Exception e)
            {   
                Console.WriteLine(e);
            }
            await Task.Delay(-1);
        }

        private async Task Client_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Message.Content == "!ping")
            {
                await e.Message.RespondAsync("Pong !");
                await e.Message.RespondAsync("*Ce message a été envoyé depuis mon client de messagerie* ***Raspberry Pi***");
            }
        }

        private static void DebugLogger_LogMessageReceived(object sender, DSharpPlus.EventArgs.DebugLogMessageEventArgs e)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //Console.Write(DateTime.Now.ToString("HH:mm:ss,ff"));
            Console.Write(e.Timestamp);

            Console.ResetColor();
            Console.Write(" ");

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(e.Application);
            Console.ResetColor();
            Console.Write(" : ");

            switch (e.Level)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    break;
            }
            Console.WriteLine(e.Message);

            Console.ResetColor();
        }
    }


    struct Config
    {
        [JsonProperty("token")]
        public string token { get; private set; }
        [JsonProperty("prefix")]
        public string prefix { get; private set; }
        [JsonProperty("logfile")]
        public string logFile { get; private set; }
    }
}
