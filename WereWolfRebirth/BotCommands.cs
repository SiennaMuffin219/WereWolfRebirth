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

namespace WereWolfRebirth
{
    class BotCommands
    {

        [Command("ping")]
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
                    var guildTask= e.Client.CreateGuildAsync("Loup Garou");
                    guild = guildTask.GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            Console.WriteLine(2);
            var village = await guild.CreateChannelAsync("Village", ChannelType.Text);
            Console.WriteLine(3);
            var loup = await guild.CreateChannelAsync("Loup", ChannelType.Text);
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
            try
            {
                Game.players = new List<DiscordUser>();


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

                        if(!Game.players.Contains(react.User))
                        {
                            Game.players.Add(react.User);
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

            await RoleAssignment(guild);

            System.Threading.Thread.Sleep((int)1E5); // 100 s avant la destruction du serveur
            Console.WriteLine(14);

            await guild.DeleteAsync();

        }


        public async Task RoleAssignment(DiscordGuild guild)
        {
            Game.personnages = Personnage.CreatePersonnages(Game.players);
            
            DiscordChannel chPerso = await guild.CreateChannelAsync("Salon Personnel", ChannelType.Category);

            foreach (var player in Game.personnages)
            {
                DiscordChannel currentCh = null;
                foreach (var otherPlayer in Game.personnages.FindAll(x => x.Me != player.Me))
                {
                    currentCh = await guild.CreateChannelAsync(player.Me.Username, ChannelType.Text, chPerso);
                    await currentCh.AddOverwriteAsync(otherPlayer.Me as DiscordMember, Permissions.None, Permissions.AccessChannels);
                }
                await currentCh.SendMessageAsync(player.ToString());

            }


        }



        [Command("stopInvite"), Aliases("stop", "sinv", "stopinv")]
        public async Task StopInvite(CommandContext ctx)
        {
            WereWolfRebirth.Game.wait = false;
            await Task.CompletedTask;
        }




        [Command("disconnect"), Aliases("dis", "dc")]
        public async Task Disconnect(CommandContext e)
        {
            await e.Client.UpdateStatusAsync(user_status: UserStatus.Invisible);
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "AlphaBot", "Disconnecting from Discord", DateTime.Now);
            await e.Client.DisconnectAsync();
            System.Threading.Thread.Sleep(1000);
            Environment.Exit(0);
        }
    }
}
