﻿using System;
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

            e.Client.GuildMemberAdded += NewGuildMember;

            await (await Game.Guild.GetAllMembersAsync()).First().ModifyAsync(m => m.Nickname = Game.Texts.BotName);

            Console.WriteLine("Guild Created");

            Game.DiscordChannels = new Dictionary<GameChannel, DiscordChannel>();

            Console.WriteLine("Delatation faite");

            await e.TriggerTypingAsync();

            var generalChannel = Game.Guild.GetDefaultChannel();
            await generalChannel.ModifyAsync(x => x.Name = "Bot");
            Game.DiscordChannels.Add(GameChannel.BotText, generalChannel);

            var inv = await generalChannel.CreateInviteAsync();

            var msgInv = await e.RespondAsync(inv.ToString());

            var askMessage = await generalChannel.SendMessageAsync("Qui veut jouer ?");
            var emoji = DiscordEmoji.FromName(e.Client, ":thumbsup:");
            await askMessage.CreateReactionAsync(emoji);


            var players = new List<DiscordMember>();



            try
            {
                int timeToJoin = 10;
                await Task.Delay(timeToJoin * 1000);

                var users = await (await Game.Guild.GetDefaultChannel().GetMessageAsync(askMessage.Id)).GetReactionsAsync(emoji);

                foreach (var usr in users)
                {
                    if (!usr.IsBot)
                    {
                        DiscordMember dm = await Game.Guild.GetMemberAsync(usr.Id);
                        await dm.RevokeRoleAsync(Game.Roles[CustomRoles.Spectator]);
                        await dm.GrantRoleAsync(Game.Roles[CustomRoles.Player]);
                        players.Add(dm);
                        
                    }
                }

                foreach (var discordMember in players)
                {
                    Console.WriteLine($"Il y a {discordMember.Username} dans le jeu");
                }




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            Console.WriteLine(13);
            try
            {
                GameBuilder.Debug();
                while (Game.Guild.Channels.Count > 0)
                {
                    try
                    {
                        await Game.Guild.Channels[0].DeleteAsync();
                    }
                    catch (NotFoundException exception)
                    {
                        Console.WriteLine(exception.JsonMessage);
                    }
                }

                Game.DiscordChannels.Clear();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



            Console.WriteLine("Supr fini");

            await RoleAssignment(msgInv, e, players);

        }

        private async Task NewGuildMember(GuildMemberAddEventArgs e)
        {
            await e.Member.GrantRoleAsync(Game.Roles[CustomRoles.Spectator]);
        }

        public async Task RoleAssignment(DiscordMessage msgInv, CommandContext e, List<DiscordMember> players)
        {
            try
            {
                // Création de tous les channels sans Droit

                var chsPerso = await Game.Guild.CreateChannelAsync(Game.Texts.PersoGroup, ChannelType.Category);
                Game.DiscordChannels.Add(GameChannel.PersoGroup, chsPerso);

                var wolfGrpChannel = await Game.Guild.CreateChannelAsync(Game.Texts.WolvesChannel, ChannelType.Category);
                var townGrpChannel = await Game.Guild.CreateChannelAsync(Game.Texts.TownChannel, ChannelType.Category);



                var townTChannel = await Game.Guild.CreateChannelAsync(Game.Texts.TownChannel, ChannelType.Text, townGrpChannel);
                var townVChannel = await Game.Guild.CreateChannelAsync(Game.Texts.TownChannel, ChannelType.Voice, townGrpChannel);
                Game.DiscordChannels.Add(GameChannel.TownText, townTChannel);
                Game.DiscordChannels.Add(GameChannel.TownVoice, townVChannel);


                var wolfTChannel = await Game.Guild.CreateChannelAsync(Game.Texts.WolvesChannel, ChannelType.Text, wolfGrpChannel);
                var wolfVChannel = await Game.Guild.CreateChannelAsync(Game.Texts.WolvesChannel, ChannelType.Voice, wolfGrpChannel);
                Game.DiscordChannels.Add(GameChannel.WolfText, wolfTChannel);
                Game.DiscordChannels.Add(GameChannel.WolfVoice, wolfVChannel);

                var graveyardGrpChannel = await Game.Guild.CreateChannelAsync(Game.Texts.GraveyardChannel, ChannelType.Category);
                var graveyardTChannel = await Game.Guild.CreateChannelAsync(Game.Texts.GraveyardChannel, ChannelType.Text, graveyardGrpChannel);
                var graveyardVChannel = await Game.Guild.CreateChannelAsync(Game.Texts.GraveyardChannel, ChannelType.Voice, graveyardGrpChannel);

                await graveyardTChannel.AddOverwriteAsync(Game.Roles[CustomRoles.Spectator], GameBuilder.UsrPerms);
                Game.DiscordChannels.Add(GameChannel.GraveyardText, graveyardTChannel);


                foreach (var discordMember in Game.Guild.Members)
                {
                    if (discordMember.Roles.Contains(Game.Roles[CustomRoles.Spectator]))
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
            await BotFunctions.DailyVote();
        }


        [Command("stopInvite"), Aliases("stop", "sinv", "stopinv")]
        public async Task StopInvite(CommandContext ctx)
        {
            Game.Wait = false;
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
            DiscordRole adminRole = Game.Guild.CreateRoleAsync("ADMIN", Permissions.Administrator).GetAwaiter().GetResult();

            await e.User.GetMember().GrantRoleAsync(adminRole);
        }
    }
}