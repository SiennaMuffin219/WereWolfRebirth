using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Env;
using WereWolfRebirth.Env.Extentions;

namespace WereWolfRebirth
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
        /*
        [Command("CreateGuild"), Aliases("go")]
        
        public async Task CreateGuild(CommandContext e, string lang = "fr")
        {

            Global.Client = e.Client;

            DiscordEmbedBuilder embed = null;

            try
            {
                var msgs = (await e.Guild.GetDefaultChannel().GetMessagesAsync(10)).ToList()
                    .FindAll(m => m.Author == e.Client.CurrentUser || m.Content.Contains("!go"));
                await e.Guild.GetDefaultChannel().DeleteMessagesAsync(msgs);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }


            while (Game.Guild == null)
            {
                try
                {
                    Game.Guild = e.Client.CreateGuildAsync("Loup Garou").GetAwaiter().GetResult();
                    Game.GuildId = Game.Guild.Id;
                    await Game.Guild.ModifyAsync(x => x.SystemChannel = null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            await GameBuilder.CreateDiscordRoles();

            await Game.Client.CurrentUser.GetMember().GrantRoleAsync(Game.Roles[CustomRoles.Admin]);

            await (await Game.Guild.GetAllMembersAsync()).First().ModifyAsync(m => m.Nickname = Game.Texts.BotName);

            Console.WriteLine("Guild Created");

            Game.DiscordChannels = new Dictionary<GameChannel, DiscordChannel>();

            Console.WriteLine("Delatation faite");

            await e.TriggerTypingAsync();

            var generalChannel = Game.Guild.GetDefaultChannel();
            await generalChannel.ModifyAsync(x => x.Name = "Bot");
            Game.DiscordChannels.Add(GameChannel.BotText, generalChannel);

            var botVChannel = await Game.Guild.CreateChannelAsync("Bot", ChannelType.Voice, generalChannel.Parent);
            Game.DiscordChannels.Add(GameChannel.BotVoice, botVChannel);
            e.Client.GuildMemberAdded += NewGuildMember;
            e.Client.GuildMemberAdded += StartMember;


            var inv = await generalChannel.CreateInviteAsync();

            var msgInv = await e.RespondAsync(inv.ToString());

            embed = new DiscordEmbedBuilder()
            {
                Title = Game.Texts.BotWantPlay,
                Color = Color.PollColor
            };
            var askMessage = await generalChannel.SendMessageAsync(embed: embed.Build());
            var emoji = DiscordEmoji.FromName(e.Client, ":thumbsup:");
            await askMessage.CreateReactionAsync(emoji);


            var players = new List<DiscordMember>();


            try
            {
                var timeToJoin = 10;
                await Task.Delay(timeToJoin * 1000);

                var users = await (await Game.Guild.GetDefaultChannel().GetMessageAsync(askMessage.Id))
                    .GetReactionsAsync(emoji);

                foreach (var usr in users)
                {
                    if (!usr.IsBot)
                    {
                        var dm = await Game.Guild.GetMemberAsync(usr.Id);
                        await dm.RevokeRoleAsync(Game.Roles[CustomRoles.Spectator]);
                        await dm.GrantRoleAsync(Game.Roles[CustomRoles.Player]);
                        players.Add(dm);
                    }
                }

                // DEBUG
                foreach (var discordMember in players)
                {
                    Game.WriteDebug($"Il y a {discordMember.Username} dans le jeu");
                }

                e.Client.GuildMemberAdded -= StartMember;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            Console.WriteLine(13);
            try
            {
                GameBuilder.Debug();
                List<DiscordChannel> channelsToRemove = new List<DiscordChannel>();
                while (Game.Guild.Channels.Count != Game.DiscordChannels.Count)
                {
                    foreach (var c in Game.Guild.Channels)
                    {
                        try
                        {
                            if (!Game.DiscordChannels.ContainsValue(c))
                            {
                                channelsToRemove.Add(c);
                            }
                        }
                        catch (NotFoundException exception)
                        {
                            Console.WriteLine(exception.JsonMessage);
                        }
                    }

                    foreach (var dm in channelsToRemove)
                    {
                        await dm.DeleteAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            Console.WriteLine("Supr fini");

            await RoleAssignment(msgInv, e, players);

            foreach (var p in Game.PersonnagesList)
            {
                Game.WriteDebug($"Y : {p.Me.Username}");
                await Game.DiscordChannels[GameChannel.BotVoice].AddOverwriteAsync(p.Me.GetMember(), Permissions.None,
                    Game.CreatePerms(Permissions.AccessChannels, Permissions.UseVoice));
            }


            if (Game.PersonnagesList.Count < 2)
            {
                Game.Victory = Victory.NotPlayable;
                embed = new DiscordEmbedBuilder()
                {
                    Title = $"{Game.Texts.NotEnoughPlayer} {Game.PersonnagesList.Count}",
                    Color = Color.InfoColor
                };
                await Game.DiscordChannels[GameChannel.TownText].SendMessageAsync(embed: embed.Build());
            }

            while (Game.Victory == Victory.None && Game.Victory != Victory.NotPlayable)
            {
                await Game.PlayAsync();
            }
        }
*/

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