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
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Net.Abstractions;
using DSharpPlus.Net;
using Newtonsoft.Json;
using WereWolfRebirth.Roles;
using WereWolfRebirth.Environment;

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
            Console.WriteLine(1);
            DiscordGuild guild = null;
            while (guild is null)
            {
                try
                {
                    var guildTask = await e.Client.CreateGuildAsync("Loup Garou");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            foreach (var ch in guild.Channels)
            {
                await ch.DeleteAsync("Faire de la place");
            }
            Console.WriteLine(2);
            var village = await guild.CreateChannelAsync("Village", ChannelType.Text);
            Console.WriteLine(3);
            Console.WriteLine(4);
            await e.TriggerTypingAsync();
            Console.WriteLine(5);
            var inv = await village.CreateInviteAsync();
            Console.WriteLine(6);

            await e.RespondAsync(inv.ToString());

            var botChannel = await guild.CreateChannelAsync("bot", ChannelType.Text);
            Console.WriteLine(7);
            DiscordMessage askMessage = await botChannel.SendMessageAsync("Qui veut jouer ?");
            Console.WriteLine(8);

            Console.WriteLine(9);
            var interact = e.Client.GetInteractivityModule();
            Console.WriteLine(10);

            DateTime end = DateTime.Now.AddSeconds(10);


            Console.WriteLine(11);
            List<DiscordUser> players = new List<DiscordUser>();
            try
            {
                while (Game.wait)
                {
                    Console.WriteLine("Boucle");
                    Console.WriteLine(end - DateTime.Now);
                    Console.WriteLine(DateTime.Now.ToString());
                    if (DateTime.Now >= end)
                    {
                        Console.WriteLine("Couper");

                        break;
                    }

                    var react = await interact.WaitForMessageReactionAsync(askMessage, null, new TimeSpan(0, 0, 10));
                    if (!(react is null))
                    {
                        Console.WriteLine(12);

                       if(!players.Contains(react.User))
                       {
                            players.Add(react.User);
                       }



                        Console.WriteLine(react.User.Username);
                        end = DateTime.Now.AddSeconds(10);
                    }
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
                
            }
            catch (System.Exception ex)
            {
                
                System.Console.WriteLine(ex);
            }
            Console.WriteLine(13.2);

            await RoleAssignment(guild, players);
            
            GameBuilder.Debug();
            Console.WriteLine(14);

            System.Threading.Thread.Sleep((int)1E5); // 100 s avant la destruction du serveur
            Console.WriteLine("DELETATION");
            
            await guild.DeleteAsync();

        }


        public async Task RoleAssignment(DiscordGuild guild, List<DiscordUser> players)
        {
            try
            {

                // for(int i = 0; i < 15; i++)
                // {
                //     players.Add(players[0]);
                // }

                

                GameBuilder.CreatePersonnages(players);
                
                DiscordChannel chsPerso = await guild.CreateChannelAsync("Salons Personnels", ChannelType.Category);
                DiscordChannel chsCommun = await guild.CreateChannelAsync("Salons Communs", ChannelType.Category);
                var chLoup = await guild.CreateChannelAsync("Loup", ChannelType.Text, chsCommun);


                foreach (var player in Game.personnages)
                {
                    DiscordChannel currentTextCh = await guild.CreateChannelAsync(player.Me.Username, ChannelType.Text, chsPerso);
                    DiscordChannel currentVoiceCh = await guild.CreateChannelAsync(player.Me.Username, ChannelType.Voice, chsPerso);
                    await currentVoiceCh.PlaceMemberAsync(player.Me as DiscordMember);
                    foreach (var otherPlayer in Game.personnages.FindAll(x => x.Me != player.Me))
                    {
                        await currentTextCh.AddOverwriteAsync(otherPlayer.Me as DiscordMember, Permissions.None, Permissions.AccessChannels);
                        await currentVoiceCh.AddOverwriteAsync(otherPlayer.Me as DiscordMember, Permissions.None, Permissions.AccessChannels);
                    }

                    await currentTextCh.SendMessageAsync(player.ToString());
                }
            }
            catch(SystemException ex)
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
