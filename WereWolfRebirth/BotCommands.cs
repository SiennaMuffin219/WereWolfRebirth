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
using Newtonsoft.Json;

namespace WereWolfRebirth
{
    class BotCommands
    {



        private static string nameOfBot = "Alpha Test 42";
        private static string messageOfBot = "Qui veux jouer ?";
        

        [Command("ping")]
        public async Task Ping(CommandContext e)
        {
            await e.TriggerTypingAsync();
            await e.RespondAsync($"{e.User.Mention} Pong ({e.Client.Ping}ms)");
        }



        [Command("init")]
        public async Task Init(CommandContext e)
        {
            // var channels = await e.Guild.GetChannelsAsync();
            // foreach(var channel in channels)
            // {
            //     if(channel.Name != "general")
            //     {
            //         await channel.DeleteAsync();
            //     }
            // }
            // Console.WriteLine(e.Member.Username);
            
            // var a = await e.Guild.CreateChannelAsync("Village", ChannelType.Text);
            

            // await e.RespondAsync(messageOfBot);
            // // On check dans les messages, le message du bot
            // var messages = await a.GetMessagesAsync(30);
            // foreach(var m in messages)
            // {   
            //     if(m.Author.Username == e.Client.CurrentUser.Username && m.Content == messageOfBot)
            //     {
            //         Console.WriteLine(m.Reactions.Count);
            //     }

            //     System.Console.WriteLine(m);
            // }


            foreach(var j in listPlayer)
            {
                if(!j.IsBot && (j.Presence.Status == UserStatus.Online  || j.Presence.Status == UserStatus.DoNotDisturb))
                {
                    await e.Guild.CreateRoleAsync(j.Username);
                    await e.Guild.CreateChannelAsync(j.Username, ChannelType.Text);
                }
            }


        }
        
        [Command("CreateGuild"), Aliases("go")]

        public async Task CreateGuild(CommandContext e)
        {
            Console.WriteLine(e);

            var guild = await e.Client.CreateGuildAsync("Loup Garou");
            var village = await guild.CreateChannelAsync("Village", ChannelType.Text);
            var loup = await guild.CreateChannelAsync("Loup", ChannelType.Text);
            await e.TriggerTypingAsync();
            var inv = await village.CreateInviteAsync();

            await e.RespondAsync(inv.ToString());

            var botChannel = await guild.CreateChannelAsync("bot", ChannelType.Text);
            DiscordMessage askMessage = await botChannel.SendMessageAsync("Qui veut jouer ?");

            var interact = e.Client.GetInteractivityModule();

            DateTime end = DateTime.Now.AddSeconds(10);


            while(WereWolfRebirth.Environnement.wait )
            {
                System.Console.WriteLine(DateTime.Now.ToString());
                if(DateTime.Now >= end)
                {
                    break;
                }       
                
                var react = await interact.WaitForMessageReactionAsync(askMessage, null, new TimeSpan(0, 0, 10));
                if(!(react is null))
                {
                    WereWolfRebirth.Environnement.players.Add(react.User);    
                    System.Console.WriteLine(react.User.Username);
                    end = DateTime.Now.AddSeconds(10);
                }
            }


            System.Threading.Thread.Sleep((int)1E5); // 100 s avant la destruction du serveur

            await guild.DeleteAsync();
            
        }

        [Command("stopInvite"), Aliases("stop", "sinv", "stopinv")]
        public async Task StopInvite(CommandContext ctx)
        {
            WereWolfRebirth.Environnement.wait = false;
            await Task.CompletedTask;
        }


        [Command("Launch"), Aliases("l")]
        public async Task LaunchGame(CommandContext e)
        {
            var listChelou = await e.Guild.GetAllMembersAsync();
            List<DiscordChannel> listChannel = new List<DiscordChannel>(); 
            foreach(var p in listChelou)
            {
                WereWolfRebirth.Environnement.players.Add((DiscordUser) p);

            }
        }



        [Command("join")]
        public async Task Join(CommandContext ctx, string channelName)
        {

        }

        [Command("night")]
        public async Task Night(CommandContext ctx)
        {
            
        }



        [Command("initSalons")]
        public async Task Init(CommandContext e, int nbChannels)
        {
            for(int i = 0; i < nbChannels; i++)
            {
                await e.Guild.CreateChannelAsync("test " + i, ChannelType.Text);
            }
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
