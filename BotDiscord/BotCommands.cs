using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotDiscord.Env;
using BotDiscord.Env.Enum;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace BotDiscord
{
    public class BotCommands : BaseCommandModule
    {
        [Command("game"), Aliases("go")]
        [Description("Available langages: 'fr', 'en', 'es', 'de', 'ja'")]

        public async Task CreateGame(CommandContext e, string lang = "fr")
        {
            
            await Task.Run(() =>
            {
                Global.Games.Add(new Game(e, lang));
                Global.Games.Last().CreateGuild(e, lang).GetAwaiter().GetResult();
            });
        }

        [Command("ping"), Description("")]
        public async Task Ping(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await e.RespondAsync($"{e.User.Mention} Pong ({e.Client.Ping}ms)");
        }


        private async Task NewGuildMember(GuildMemberAddEventArgs e)
        {
            await e.Member.GrantRoleAsync(Global.Roles[CustomRoles.Spectator]);
        }

        private async Task StartMember(GuildMemberAddEventArgs e)
        {
            Permissions p = GameBuilder.CreatePerms(Permissions.AccessChannels, Permissions.UseVoice, Permissions.Speak);
            await Global.Games[Global.currGame].DiscordChannels[GameChannel.BotVoice].AddOverwriteAsync(e.Member, p);
            Game.WriteDebug($"D : {e.Member.Username}");
        }


        [Command("disconnect"), Aliases("dis", "dc")]
        public async Task Disconnect(CommandContext e)
        {
            await e.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible);
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "AlphaBot", "Disconnecting from Discord", DateTime.Now);
            await e.Client.DisconnectAsync();
            Thread.Sleep(1000);
            Environment.Exit(0);
        }


        [Command("test")]
        public async Task Test(CommandContext e)
        {
            await Task.Run(() => Game.WriteDebug("test"));
        }

        [Command("delete")]
        public async Task Delete(CommandContext e)
        {
            foreach (var discordChannel in e.Guild.Channels)
            {
                await discordChannel.DeleteAsync();
            }
        }

        [Command("admin")]
        public async Task AdminTask(CommandContext e)
        {
            var adminRole = e.Guild.CreateRoleAsync("ADMIN", Permissions.Administrator).GetAwaiter().GetResult();

            await GameBuilder.GetMember(e.Guild, e.User).GrantRoleAsync(adminRole);
        }
    }
}