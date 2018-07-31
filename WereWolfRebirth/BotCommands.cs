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
            var channels = await e.Guild.GetChannelsAsync();
            foreach(var channel in channels)
            {
                if(channel.Name != "general")
                {
                    await channel.DeleteAsync();
                }
            }
            Console.WriteLine(e.Member.Username);
            
            var a = await e.Guild.CreateChannelAsync("Village", ChannelType.Text);
            

            await e.RespondAsync(messageOfBot);
            // On check dans les messages, le message du bot
            var messages = await a.GetMessagesAsync();
            foreach(var m in messages)
            {   
                if(m.Author.Username == nameOfBot && m.Content == messageOfBot)
                {
                    Console.WriteLine(m.Reactions.Count);
                }
            }


            var listChelou = await e.Guild.GetAllMembersAsync(); 
            List<DiscordUser> listPlayer = new List<DiscordUser>();
            foreach(var player in listChelou)
            {
                listPlayer.Add((DiscordUser) player);
            }
            


            foreach(var j in listPlayer)
            {
                if(!j.IsBot && (j.Presence.Status == UserStatus.Online  || j.Presence.Status == UserStatus.DoNotDisturb))
                {
                    await e.Guild.CreateRoleAsync(j.Username);
                    await e.Guild.CreateChannelAsync(j.Username, ChannelType.Text);
                }
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

    }
}
