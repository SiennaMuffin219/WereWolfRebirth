using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Env;
using WereWolfRebirth.Roles;

namespace WereWolfRebirth
{
    public class BotCommands : BaseCommandModule
    {

        [Command("ping"), Description("")]
        public async Task Ping(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await e.RespondAsync($"{e.User.Mention} Pong ({e.Client.Ping}ms)");
        }

        [Command("CreateGuild"), Aliases("go")]
        [Description("Available langages: 'fr', 'en', 'es', 'de', 'ja'")]
        public async Task CreateGuild(CommandContext e, string lang = "fr")
        {

            Game.SetLanguage(lang);

            Game.Client = e.Client;
            Console.WriteLine(1);
            while (Game.guild == null)
            {
                try
                {
                    Game.guild = e.Client.CreateGuildAsync("Loup Garou").GetAwaiter().GetResult();
                    Game.GuildId = Game.guild.Id;
                    await Game.guild.ModifyAsync(x => x.SystemChannel = null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine("Guild Created");

            Game.DiscordChannels = new Dictionary<GameChannel, DiscordChannel>();

            Console.WriteLine("Delatation faite");
     
            await e.TriggerTypingAsync();

            var generalChannel = Game.guild.GetDefaultChannel();
            await generalChannel.ModifyAsync(x => x.Name = "Bot");
            Game.DiscordChannels.Add(GameChannel.BotText, generalChannel);

            var inv = await generalChannel.CreateInviteAsync();

            var msgInv = await e.RespondAsync(inv.ToString());
           
            var askMessage = await generalChannel.SendMessageAsync("Qui veut jouer ?");
			await askMessage.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":thumbsup:"));

            var interact = e.Client.GetInteractivity();

            var end = DateTime.Now.AddSeconds(10);


            var players = new List<DiscordUser>();
            DiscordRole spectRole = null;
            
            try
            {
                while (Game.wait)
                {
                    Console.WriteLine("Boucle");

                    if (DateTime.Now >= end)
                    {
                        Console.WriteLine("Couper");

                        break;
                    }

                    var react = await interact.WaitForMessageReactionAsync(askMessage, null, new TimeSpan(0, 0, 10));
                    if (!(react is null))
                    {

                        if (!players.Contains(react.User) && !react.User.IsBot)
                        {
                            players.Add(react.User);
                            end = DateTime.Now.AddSeconds(10);
                            Console.WriteLine(react.User.Username);
                        }
                    }
                }


                #region Roles

                var adminRole = await Game.guild.CreateRoleAsync("Bot", Permissions.Administrator,
                    new DiscordColor("#EE0000"), true, true, "Role Bot");

                var playerPerms = Game.CreatePerms(Permissions.SendMessages, Permissions.ReadMessageHistory, Permissions.AddReactions);

                var playerRole = await Game.guild.CreateRoleAsync("Joueur", playerPerms,
                    new DiscordColor("#1de020"), true, true, "Role Joueur");


                var spectPerms = Game.CreatePerms(Permissions.AccessChannels, Permissions.ReadMessageHistory);
                spectRole = await Game.guild.CreateRoleAsync("Spectateur", spectPerms,
                    new DiscordColor("#7200a3"), true, false, "Role spectateur");


                await Game.guild.EveryoneRole.ModifyAsync(x => x.Permissions = Permissions.None);


                foreach (var member in await Game.guild.GetAllMembersAsync())
                {
                    if (players.Contains(member))
                    {
                        await member.GrantRoleAsync(playerRole);
                    }
                    else if (member == Game.guild.CurrentMember)
                    {
                        await Game.guild.CurrentMember.GrantRoleAsync(adminRole);
                    }
                    else
                    {
                        await member.GrantRoleAsync(spectRole);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            Console.WriteLine(13);
            try
            {
                GameBuilder.Debug();
                while (Game.guild.Channels.Count > 0)
                {
                    await Game.guild.Channels[0].DeleteAsync();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


      
            Console.WriteLine("Supr fini");

            await RoleAssignment(msgInv, e, players, spectRole);

        }


        public async Task RoleAssignment(DiscordMessage msgInv, CommandContext e, List<DiscordUser> players, DiscordRole spectRole)
        {
            try
            {
                // Création de tous les channels sans Droit

                var chsPerso = await Game.guild.CreateChannelAsync(Game.TextJson.PersoGroup, ChannelType.Category);
                var chsCommun = await Game.guild.CreateChannelAsync(Game.TextJson.SharedGroup, ChannelType.Category);
                Game.DiscordChannels.Add(GameChannel.SharedGroup, chsCommun);
                Game.DiscordChannels.Add(GameChannel.PersoGroup, chsPerso);

                var townTChannel = await Game.guild.CreateChannelAsync(Game.TextJson.TownChannel, ChannelType.Text, chsCommun);
                var townVChannel = await Game.guild.CreateChannelAsync(Game.TextJson.TownChannel, ChannelType.Voice, chsCommun);
                Game.DiscordChannels.Add(GameChannel.TownText, townTChannel);
                Game.DiscordChannels.Add(GameChannel.TownVoice, townVChannel);
                
                
                var wolfTChannel = await Game.guild.CreateChannelAsync(Game.TextJson.WolvesChannel, ChannelType.Text, chsCommun);
                var wolfVChannel = await Game.guild.CreateChannelAsync(Game.TextJson.WolvesChannel, ChannelType.Voice, chsCommun);
                Game.DiscordChannels.Add(GameChannel.WolfText, wolfTChannel);
                Game.DiscordChannels.Add(GameChannel.WolfVoice, wolfVChannel);

                var graveyardTChannel = await Game.guild.CreateChannelAsync(Game.TextJson.GraveyardChannel, ChannelType.Text, chsCommun);
                var graveyardVChannel = await Game.guild.CreateChannelAsync(Game.TextJson.GraveyardChannel, ChannelType.Voice, chsCommun);
                await graveyardTChannel.AddOverwriteAsync(spectRole, GameBuilder.UsrPerms);
                Game.DiscordChannels.Add(GameChannel.GraveyardVoice, graveyardVChannel);
                Game.DiscordChannels.Add(GameChannel.GraveyardText, graveyardTChannel);


                foreach (var discordMember in Game.guild.Members)
                {
                    if (discordMember.Roles.Contains(spectRole))
                    {
                        await graveyardVChannel.AddOverwriteAsync(discordMember, Game.CreatePerms(Permissions.UseVoiceDetection, Permissions.UseVoice, Permissions.Speak));
                    }
                }

                await GameBuilder.CreatePersonnages(players);

                await (await e.Channel.GetMessageAsync(msgInv.Id)).ModifyAsync((await townTChannel.CreateInviteAsync()).ToString());


            

            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex);
            }
        }


        [Command("vote")]
        public async Task DailyVote(CommandContext e)
        {
            await BotFunctions.DailyVote(e);
        }


        [Command("stopInvite"), Aliases("stop", "sinv", "stopinv")]
        public async Task StopInvite(CommandContext ctx)
        {
            Game.wait = false;
            await Task.CompletedTask;
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
        DiscordRole AdminRole = Game.guild.CreateRoleAsync("ADMIN", Permissions.Administrator).GetAwaiter().GetResult();

        await(e.User as DiscordMember).GrantRoleAsync(AdminRole);
        }
    }
}