using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace WereWolfRebirth
{
    internal class Program
    {
        private DiscordClient _client;
        private CommandsNextExtension _commands;
        private static Config _config;
        private static void Main(string[] args) => new Program().AsyncMain().GetAwaiter().GetResult();

        public async Task AsyncMain()
        {
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("../../config.json"));
            _client = new DiscordClient(new DiscordConfiguration {LogLevel = LogLevel.Debug, Token = _config.Token});
            _commands = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                StringPrefixes = new List<string> {_config.Prefix}
            });
            _commands.RegisterCommands<BotCommands>();

            _client.DebugLogger.LogMessageReceived += DebugLogger_LogMessageReceived;
            _client.ChannelCreated += Client_ChannelCreated;
            _client.ChannelDeleted += Client_ChannelDeleted;
            _client.ChannelPinsUpdated += Client_ChannelPinsUpdated;
            _client.ChannelUpdated += Client_ChannelUpdated;
            _client.ClientErrored += Client_ClientErrored;
            _client.DmChannelCreated += Client_DmChannelCreated;
            _client.DmChannelDeleted += Client_DmChannelDeleted;
            _client.GuildAvailable += Client_GuildAvailable;
            _client.GuildBanAdded += Client_GuildBanAdded;
            _client.GuildBanRemoved += Client_GuildBanRemoved;
            _client.GuildCreated += Client_GuildCreated;
            _client.GuildDeleted += Client_GuildDeleted;
            _client.GuildEmojisUpdated += Client_GuildEmojisUpdated;
            _client.GuildIntegrationsUpdated += Client_GuildIntegrationsUpdated;
            _client.GuildMemberAdded += Client_GuildMemberAdded;
            _client.GuildMemberRemoved += Client_GuildMemberRemoved;
            _client.GuildMembersChunked += Client_GuildMembersChunked;
            _client.GuildMemberUpdated += Client_GuildMemberUpdated;
            _client.GuildRoleCreated += Client_GuildRoleCreated;
            _client.GuildRoleDeleted += Client_GuildRoleDeleted;
            _client.GuildRoleUpdated += Client_GuildRoleUpdated;
            _client.GuildUnavailable += Client_GuildUnavailable;
            _client.GuildUpdated += Client_GuildUpdated;
            _client.Heartbeated += Client_Heartbeated;
            _client.MessageAcknowledged += Client_MessageAcknowledged;
            _client.MessageCreated += Client_MessageCreated;
            _client.MessageDeleted += Client_MessageDeleted;
            _client.MessageReactionAdded += Client_MessageReactionAdded;
            _client.MessageReactionRemoved += Client_MessageReactionRemoved;
            _client.MessageReactionsCleared += Client_MessageReactionsCleared;
            _client.MessagesBulkDeleted += Client_MessagesBulkDeleted;
            _client.MessageUpdated += Client_MessageUpdated;
            _client.PresenceUpdated += Client_PresenceUpdated;
            _client.Ready += Client_Ready;
            _client.Resumed += Client_Resumed;
            _client.SocketClosed += Client_SocketClosed;
            _client.SocketErrored += Client_SocketErrored;
            _client.SocketOpened += Client_SocketOpened;
            _client.TypingStarted += Client_TypingStarted;
            _client.UnknownEvent += Client_UnknownEvent;
            _client.UserSettingsUpdated += Client_UserSettingsUpdated;
            _client.UserUpdated += Client_UserUpdated;
            _client.VoiceServerUpdated += Client_VoiceServerUpdated;
            _client.VoiceStateUpdated += Client_VoiceStateUpdated;
            _client.WebhooksUpdated += Client_WebhooksUpdated;
            try
            {
                await _client.ConnectAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.Delay(-1);
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            if (e.Guild.Name == "Loup Garou" && e.Guild.JoinedAt < DateTimeOffset.Now.AddSeconds(-10))
                e.Guild.DeleteAsync();
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Guild.Name} is now available", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ChannelCreated(ChannelCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Channel.Name} created in {e.Guild.Name}",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_WebhooksUpdated(WebhooksUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp",
                $"Webhooks updated in {e.Channel.Name} ({e.Guild.Name})", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_VoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            try
            {
                e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp",
                    $"{e.User.Username} updated voice in {e.Channel.Name} ({e.Guild.Name})", DateTime.Now);
            }
            catch (Exception)
            {
                Console.WriteLine();
            }

            return Task.CompletedTask;
        }

        private Task Client_VoiceServerUpdated(VoiceServerUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp",
                $"{e.Guild.Name} updated voice server to {e.Endpoint}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_UserUpdated(UserUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.UserAfter.Username} just updated",
                DateTime.Now);
            e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp",
                $"{e.UserBefore.Username}#{e.UserBefore.Discriminator} => {e.UserAfter.Username}#{e.UserAfter.Discriminator}",
                DateTime.Now);
            e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp",
                $"{e.UserBefore.AvatarUrl} => {e.UserAfter.AvatarUrl}", DateTime.Now);
            e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp",
                $"{e.UserBefore.Presence.Activity.Name} => {e.UserAfter.Presence.Activity.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_UserSettingsUpdated(UserSettingsUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_UnknownEvent(UnknownEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_TypingStarted(TypingStartEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_SocketOpened()
        {
            //e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_SocketErrored(SocketErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_SocketClosed(SocketCloseEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_Resumed(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_PresenceUpdated(PresenceUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_MessageUpdated(MessageUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_MessagesBulkDeleted(MessageBulkDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_MessageReactionsCleared(MessageReactionsClearEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_MessageReactionRemoved(MessageReactionRemoveEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_MessageReactionAdded(MessageReactionAddEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_MessageDeleted(MessageDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_MessageCreated(MessageCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_MessageAcknowledged(MessageAcknowledgeEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_Heartbeated(HeartbeatEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildUpdated(GuildUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildUnavailable(GuildDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Guild.Name} deleted", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildRoleUpdated(GuildRoleUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildRoleDeleted(GuildRoleDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Role.Name} deleted on {e.Guild.Name})",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildRoleCreated(GuildRoleCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Role.Name} created on {e.Guild.Name})",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildMemberUpdated(GuildMemberUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildMembersChunked(GuildMembersChunkEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Member.Username} left {e.Guild.Name})",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Member.Username} added on {e.Guild.Name})",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildIntegrationsUpdated(GuildIntegrationsUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildEmojisUpdated(GuildEmojisUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildDeleted(GuildDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Guild.Name} deleted", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildCreated(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Guild.Name} created", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildBanRemoved(GuildBanRemoveEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildBanAdded(GuildBanAddEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_DmChannelDeleted(DmChannelDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_DmChannelCreated(DmChannelCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ClientErrored(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "BotApp", $"Client error: {e.EventName}  -  {e.Exception}",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ChannelUpdated(ChannelUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.ChannelBefore.Name} just updated",
                DateTime.Now);
            if (e.ChannelBefore.IsNSFW != e.ChannelAfter.IsNSFW)
                e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp",
                    $"{e.ChannelBefore.Name} is {(e.ChannelAfter.IsNSFW ? "now" : "no longer")} NSFW", DateTime.Now);
            if (e.ChannelBefore.IsPrivate != e.ChannelAfter.IsPrivate)
                e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp",
                    $"{e.ChannelBefore.Name} is {(e.ChannelAfter.IsPrivate ? "now" : "no longer")} private",
                    DateTime.Now);
            if (e.ChannelBefore.Name != e.ChannelAfter.Name)
                e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp",
                    $"{e.ChannelBefore.Name} is now named {e.ChannelAfter.Name}", DateTime.Now);
            if (e.ChannelBefore.Topic != e.ChannelAfter.Topic)
                e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp",
                    $"{e.ChannelBefore.Name}'s topic is now {e.ChannelAfter.Topic}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ChannelPinsUpdated(ChannelPinsUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp",
                $"New pin on {e.Channel.Name} ({e.Channel.Guild.Name})", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ChannelDeleted(ChannelDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Channel.Name} deleted on {e.Guild.Name}",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private static void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            if (e.Application == "REST")
                return;

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(DateTime.Now.ToString("HH:mm:ss,ff"));
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
            }

            Console.WriteLine(e.Message);
            //File.AppendAllText(_config.LogFile, $"[{e.Level}] [{e.Timestamp}] [{e.Application}] : {e.Message}\n");

            Console.ResetColor();

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress broadcast = IPAddress.Parse("192.168.1.21");

            byte[] sendbuf = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new LogMessage { Loglevel = e.Level.ToString(), Message = e.Message, Source = e.Application, Timestamp = e.Timestamp}));
            IPEndPoint ep = new IPEndPoint(broadcast, 42915);

            s.SendTo(sendbuf, ep);
        }
    }

    internal struct Config
    {
        [JsonProperty("token")] public string Token { get; private set; }
        [JsonProperty("prefix")] public string Prefix { get; private set; }
        [JsonProperty("logfile")] public string LogFile { get; private set; }
    }

    struct LogMessage
    {
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("loglevel")]
        public string Loglevel { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}