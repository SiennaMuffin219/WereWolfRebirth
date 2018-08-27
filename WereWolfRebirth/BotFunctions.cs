using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WereWolfRebirth.Env;
using WereWolfRebirth.Roles;

namespace WereWolfRebirth
{
    class BotFunctions
    {
        public static DiscordMessage DailyVotingMessage;

        
        public static async Task DailyVote(CommandContext e)
        {
        
            Game.VoteList = new List<Vote>();
     
            DailyVotingMessage = await Game.DiscordChannels["villageText"].SendMessageAsync(Game.TextJson.DailyVoteMessage);

            foreach (Personnage personnage in Game.PersonnagesList)
            {
                Console.WriteLine($"Personnage : {personnage.Me.Username} -> {personnage.Emoji.Name}");
                await DailyVotingMessage.CreateReactionAsync(personnage.Emoji);
            }

            Game.Client.MessageReactionAdded += ClientOnMessageReactionAdded;

        }

        private static async Task ClientOnMessageReactionAdded(MessageReactionAddEventArgs e)
        {
            Console.WriteLine("pute");

            if (!e.User.IsBot)
            {
                Vote newVote = new Vote(e.User.Username, e.Emoji.Name);



                foreach (DiscordGuildEmoji otherEmoji in (await Game.guild.GetEmojisAsync()))
                {
                    if (otherEmoji.Name != e.Emoji.Name)
                    {
                        await DailyVotingMessage.DeleteReactionAsync(otherEmoji, e.User,
                            $"{e.User.Username} already voted");
                    }
                }
            }

        }
    }
}
