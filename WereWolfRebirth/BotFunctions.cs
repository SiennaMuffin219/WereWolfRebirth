using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Env;

namespace WereWolfRebirth
{
    internal class BotFunctions
    {
        public static DiscordMessage DailyVotingMessage;
        public static readonly int TimeToVote = 10;

        public static async Task DailyVote(CommandContext e)
        {
            Game.VoteList = new List<Vote>();

            DailyVotingMessage = await Game.DiscordChannels[GameChannel.TownText]
                .SendMessageAsync(Game.TextJson.DailyVoteMessage);

            var startTime = DateTime.Now;

            foreach (var personnage in Game.PersonnagesList.FindAll(personnage => personnage.Alive))
            {
                Console.WriteLine($"Personnage : {personnage.Me.Username} -> {personnage.Emoji.Name}");
                await DailyVotingMessage.CreateReactionAsync(personnage.Emoji);
            }

            Game.Client.MessageReactionAdded += ClientOnMessageReactionAdded;

            while (DateTime.Now < startTime.AddSeconds(TimeToVote))
            {
                await Task.Delay(500);
            }

            Console.WriteLine("Le temps est fini");

            var max = 1;

            foreach (var react in DailyVotingMessage.Reactions)
            {
                if (max < react.Count)
                {
                    max = react.Count;
                }
            }

            var emoji = DailyVotingMessage.Reactions.First(reaction => reaction.Count == max).Emoji;

            await Game.SetSpectatorAsync(Game.PersonnagesList.Find(personnage => personnage.Emoji == emoji));
        }

        private static async Task ClientOnMessageReactionAdded(MessageReactionAddEventArgs e)
        {
            Console.WriteLine("pute");

            bool present = false;
            foreach (var personnage in Game.PersonnagesList.FindAll(p => p.Alive))
            {
                if (e.Emoji == personnage.Emoji)
                {
                    present = true;
                }
            }

            if (!present)
            {
                await DailyVotingMessage.DeleteReactionAsync(e.Emoji, e.User);
                return;
            }


            if (!e.User.IsBot)
            {
                var newVote = new Vote(e.User.Username, e.Emoji.Name);

                Game.VoteList.RemoveAll(vote => vote.Owner == newVote.Owner);
                Game.VoteList.Add(newVote);
                foreach (var otherEmoji in (await Game.guild.GetEmojisAsync()))
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