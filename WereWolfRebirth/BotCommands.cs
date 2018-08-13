using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WereWolfRebirth.Env;
using WereWolfRebirth.Roles;

namespace WereWolfRebirth
{
    class BotCommands
    {

        [Command("ping"), Description("")]
        public async Task Ping(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await e.RespondAsync($"{e.User.Mention} Pong ({e.Client.Ping}ms)");
        }


        [Command("CreateGuild"), Aliases("go")]
        public async Task CreateGuild(CommandContext e)
        {
            Game.Client = e.Client;
            Console.WriteLine(1);
            while (Game.guild == null)
            {
                try
                {
                    Game.guild = e.Client.CreateGuildAsync("Loup Garou").GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine("Guild Created");


            Game.DiscordChannels = new Dictionary<string, DiscordChannel>();

            Console.WriteLine("Delatation faite");


      
            await e.TriggerTypingAsync();




            DiscordChannel generalChannel = Game.guild.GetDefaultChannel();
            await generalChannel.ModifyAsync("Bot");
            Game.DiscordChannels.Add("bot", generalChannel);

            DiscordInvite inv = await generalChannel.CreateInviteAsync();

            DiscordMessage msgInv = await e.RespondAsync(inv.ToString());
           
            DiscordMessage askMessage = await generalChannel.SendMessageAsync("Qui veut jouer ?");

            var interact = e.Client.GetInteractivityModule();

            DateTime end = DateTime.Now.AddSeconds(10);


            List<DiscordUser> players = new List<DiscordUser>();
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

                    ReactionContext react = await interact.WaitForMessageReactionAsync(askMessage, null, new TimeSpan(0, 0, 10));
                    if (!(react is null))
                    {

                        if (!players.Contains(react.User))
                        {
                            players.Add(react.User);
                            end = DateTime.Now.AddSeconds(10);
                            Console.WriteLine(react.User.Username);
                        }
                    }
                }


                #region Roles

                DiscordRole adminRole = await Game.guild.CreateRoleAsync("Bot", Permissions.Administrator,
                    new DiscordColor("#EE0000"), true, true, "Role Bot");

                Permissions playerPerms = Game.CreatePerms(Permissions.SendMessages, Permissions.ReadMessageHistory, Permissions.AddReactions);

                DiscordRole playerRole = await Game.guild.CreateRoleAsync("Joueur", playerPerms,
                    new DiscordColor("#1de020"), true, true, "Role Joueur");


                Permissions spectPerms = Game.CreatePerms(Permissions.AccessChannels, Permissions.ReadMessageHistory);
                DiscordRole spectRole = await Game.guild.CreateRoleAsync("Spectateur", spectPerms,
                    new DiscordColor("#7200a3"), true, false, "Role spectateur");

                await Game.guild.UpdateRoleAsync(Game.guild.EveryoneRole, permissions: Permissions.AccessChannels);


                foreach (DiscordMember member in await Game.guild.GetAllMembersAsync())
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
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }


            while (Game.guild.Channels.Count > 0)
            {
                Console.WriteLine(Game.guild.Channels[0].Name);
                await Game.guild.Channels[0].DeleteAsync();
            }

            await RoleAssignment(msgInv, e, players);
            await CreateEmojies(e,players);
        }


        public async Task CreateEmojies(CommandContext e,List<DiscordUser>  players)
        {


        }


        //public async Task AskVote(CommandContext e)
        //{
        //    DiscordMessage ask = Game.guild
        //    e.Client.GetInteractivityModule().CollectReactionsAsync();
        //}


        public async Task RoleAssignment(DiscordMessage msgInv, CommandContext e, List<DiscordUser> players)
        {
      
            try
            {
                Console.WriteLine("RA : 1");
                GameBuilder.CreatePersonnages(players);

                Console.WriteLine("RA : 2");
                DiscordChannel chsPerso = await Game.guild.CreateChannelAsync("Salons Personnels", ChannelType.Category);
                DiscordChannel chsCommun = await Game.guild.CreateChannelAsync("Salons Communs", ChannelType.Category);

                Console.WriteLine("RA : 3");
                Game.DiscordChannels.Add("grpCommun", chsCommun);
                Game.DiscordChannels.Add("grpPerso", chsPerso);



                Console.WriteLine("RA : 4");
                DiscordChannel village = await Game.guild.CreateChannelAsync("Village", ChannelType.Text, chsCommun);
                DiscordChannel villageVoice = await Game.guild.CreateChannelAsync("Village", ChannelType.Voice, chsCommun);
                Game.DiscordChannels.Add("villageText", village);
                Game.DiscordChannels.Add("villageVoice", villageVoice);


                Console.WriteLine("RA : 5");
                await (await e.Channel.GetMessageAsync(msgInv.Id)).ModifyAsync((await village.CreateInviteAsync()).ToString());


                Console.WriteLine("RA : 6");

                var chLoupText = await Game.guild.CreateChannelAsync("Loup", ChannelType.Text, chsCommun);
                var chLoupVocal = await Game.guild.CreateChannelAsync("Loup", ChannelType.Voice, chsCommun);


                foreach (var player in Game.PersonnagesList)
                {
                    DiscordChannel currentTextCh =
                        await Game.guild.CreateChannelAsync(player.Me.Username, ChannelType.Text, chsPerso);

                    DiscordChannel currentVoiceCh =
                        await Game.guild.CreateChannelAsync(player.Me.Username, ChannelType.Voice, chsPerso);

                    await currentVoiceCh.PlaceMemberAsync(player.Me as DiscordMember);

                    foreach (var otherPlayer in Game.PersonnagesList.FindAll(x => x.Me != player.Me))
                    {
                        await currentTextCh.AddOverwriteAsync(otherPlayer.Me as DiscordMember, Permissions.None,
                            Permissions.AccessChannels);
                        await currentVoiceCh.AddOverwriteAsync(otherPlayer.Me as DiscordMember, Permissions.None,
                            Permissions.AccessChannels);
                    }

                    await currentTextCh.SendMessageAsync(player.ToString());
                }


                // On rend innacessible le channel au non loup
                foreach (var player in Game.PersonnagesList)
                {
                    if (!(player.GetType() ==
                          Type.GetType("WereWolfRebirth.Roles.Wolf") ||
                          player.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Wolf"))))
                    {
                        await chLoupText.AddOverwriteAsync(player.Me as DiscordMember, Permissions.None,
                            Permissions.AccessChannels);
                    }
                }
            }
            catch (SystemException ex)
            {
                System.Console.WriteLine(ex);
            }
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
            await e.Client.UpdateStatusAsync(user_status: UserStatus.Invisible);
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "AlphaBot", "Disconnecting from Discord", DateTime.Now);
            await e.Client.DisconnectAsync();
            System.Threading.Thread.Sleep(1000);
            System.Environment.Exit(0);
        }
    }
}