using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WereWolfRebirth.Env;
using WereWolfRebirth.Roles;

namespace WereWolfRebirth
{
    class BotFunctions
    {
        private static DiscordMessage DailyVotingMessage = null;

        public async Task DailyVote(CommandContext e)
        {
            if (Game.VoteList is null)
            {
                Game.VoteList = new List<Vote>();
            }
            else
            {
                Game.VoteList.Clear();
            }

            
            DiscordMessage msg = await Game.DiscordChannels["village"].SendMessageAsync(Game.TextJson.DailyVoteMessage);

            foreach (Personnage personnage in Game.PersonnagesList)
            {
                await msg.CreateReactionAsync(personnage.Emoji);
            }

            Game.Client.MessageReactionAdded += ClientOnMessageReactionAdded;




        }

        private async Task ClientOnMessageReactionAdded(MessageReactionAddEventArgs e)
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
