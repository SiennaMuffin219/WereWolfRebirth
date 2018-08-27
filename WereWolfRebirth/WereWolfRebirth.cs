using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WereWolfRebirth
{
    class Program
    {
        private DiscordClient client;
        private CommandsNextModule commands;



        static void Main(string[] args) => new Program().AsyncMain().GetAwaiter().GetResult();


        public async Task AsyncMain()
        {
            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("../../config.json"));

            client = new DiscordClient(new DiscordConfiguration()
            {
                LogLevel = LogLevel.Debug,
                Token = config.token,
            });

            commands = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                StringPrefix = config.prefix
            });
            commands.RegisterCommands<BotCommands>();
            client.UseInteractivity(new InteractivityConfiguration());

            client.DebugLogger.LogMessageReceived += DebugLogger_LogMessageReceived;
            client.ChannelCreated += Client_ChannelCreated;
            client.ChannelDeleted += Client_ChannelDeleted;
            client.ChannelPinsUpdated += Client_ChannelPinsUpdated;
            client.ChannelUpdated += Client_ChannelUpdated;
            client.ClientErrored += Client_ClientErrored;
            client.DmChannelCreated += Client_DmChannelCreated;
            client.DmChannelDeleted += Client_DmChannelDeleted;
            client.GuildAvailable += Client_GuildAvailable;
            client.GuildBanAdded += Client_GuildBanAdded;
            client.GuildBanRemoved += Client_GuildBanRemoved;
            client.GuildCreated += Client_GuildCreated;
            client.GuildDeleted += Client_GuildDeleted;
            client.GuildEmojisUpdated += Client_GuildEmojisUpdated;
            client.GuildIntegrationsUpdated += Client_GuildIntegrationsUpdated;
            client.GuildMemberAdded += Client_GuildMemberAdded;
            client.GuildMemberRemoved += Client_GuildMemberRemoved;
            client.GuildMembersChunked += Client_GuildMembersChunked;
            client.GuildMemberUpdated += Client_GuildMemberUpdated;
            client.GuildRoleCreated += Client_GuildRoleCreated;
            client.GuildRoleDeleted += Client_GuildRoleDeleted;
            client.GuildRoleUpdated += Client_GuildRoleUpdated;
            client.GuildUnavailable += Client_GuildUnavailable;
            client.GuildUpdated += Client_GuildUpdated;
            client.Heartbeated += Client_Heartbeated;
            client.MessageAcknowledged += Client_MessageAcknowledged;
            client.MessageCreated += Client_MessageCreated;
            client.MessageDeleted += Client_MessageDeleted;
            client.MessageReactionAdded += Client_MessageReactionAdded;
            client.MessageReactionRemoved += Client_MessageReactionRemoved;
            client.MessageReactionsCleared += Client_MessageReactionsCleared;
            client.MessagesBulkDeleted += Client_MessagesBulkDeleted;
            client.MessageUpdated += Client_MessageUpdated;
            client.PresenceUpdated += Client_PresenceUpdated;
            client.Ready += Client_Ready;
            client.Resumed += Client_Resumed;
            client.SocketClosed += Client_SocketClosed;
            client.SocketErrored += Client_SocketErrored;
            client.SocketOpened += Client_SocketOpened;
            client.TypingStarted += Client_TypingStarted;
            client.UnknownEvent += Client_UnknownEvent;
            client.UserSettingsUpdated += Client_UserSettingsUpdated;
            client.UserUpdated += Client_UserUpdated;
            client.VoiceServerUpdated += Client_VoiceServerUpdated;
            client.VoiceStateUpdated += Client_VoiceStateUpdated;
            client.WebhooksUpdated += Client_WebhooksUpdated;

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

        #region LOGS

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            if (e.Guild.Name == "Loup Garou" && e.Guild.JoinedAt < DateTimeOffset.Now.AddSeconds(-10))
                e.Guild.DeleteAsync();
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Guild.Name} is now available", DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_ChannelCreated(ChannelCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Channel.Name} created in {e.Guild.Name}", DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_WebhooksUpdated(WebhooksUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"Webhooks updated in {e.Channel.Name} ({e.Guild.Name})", DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_VoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            try
            {
                e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.User.Username} updated voice in {e.Channel.Name} ({e.Guild.Name})", DateTime.Now);
            }
            catch (Exception) 
            {
                System.Console.WriteLine();
            }
            return Task.CompletedTask;
        }
        private Task Client_VoiceServerUpdated(VoiceServerUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.Guild.Name} updated voice server to {e.Endpoint}", DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_UserUpdated(UserUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", $"{e.UserAfter.Username} just updated", DateTime.Now);
            e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp", $"{e.UserBefore.Username}#{e.UserBefore.Discriminator} => {e.UserAfter.Username}#{e.UserAfter.Discriminator}", DateTime.Now);
            e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp", $"{e.UserBefore.AvatarUrl} => {e.UserAfter.AvatarUrl}", DateTime.Now);
            e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp", $"{e.UserBefore.Presence.Game.Name} => {e.UserAfter.Presence.Game.Name}", DateTime.Now);
            e.Client.DebugLogger.LogMessage(LogLevel.Debug, "BotApp", $"{e.UserBefore.Presence.Game.Name} => {e.UserAfter.Presence.Game.Name}", DateTime.Now);
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
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_GuildRoleUpdated(GuildRoleUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_GuildRoleDeleted(GuildRoleDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_GuildRoleCreated(GuildRoleCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
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
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
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
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_GuildCreated(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
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
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_ChannelUpdated(ChannelUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_ChannelPinsUpdated(ChannelPinsUpdateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
        }
        private Task Client_ChannelDeleted(ChannelDeleteEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "BotApp", e.ToString(), DateTime.Now);
            return Task.CompletedTask;
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
        #endregion



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

