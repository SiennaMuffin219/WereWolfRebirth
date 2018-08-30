using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Env;
using WereWolfRebirth.Env.Extentions;
using WereWolfRebirth.Roles;

namespace WereWolfRebirth
{
    internal class BotFunctions
    {
        public static DiscordMessage DailyVotingMessage;
        public static readonly int TimeToVote = 10;

        public static async Task DailyVote()
        {

            DailyVotingMessage = await Game.DiscordChannels[GameChannel.TownText]
                .SendMessageAsync(Game.Texts.DailyVoteMessage);

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
            DailyVotingMessage = await Game.DiscordChannels[GameChannel.TownText].GetMessageAsync(DailyVotingMessage.Id);
            foreach (var discordReaction in DailyVotingMessage.Reactions)
            {
                Console.WriteLine($"Reaction : {discordReaction.Emoji.Name} : {discordReaction.Count}");
            }
            try
            {
                var emoji = DailyVotingMessage.Reactions.First(x => x.Count == DailyVotingMessage.Reactions.Max(y => y.Count)).Emoji;
                Personnage p = Game.PersonnagesList.Find(personnage => personnage.Emoji.Id == emoji.Id);
                await MakeDeath(p);

                await p.ChannelT.SendMessageAsync(Game.Texts.DeadMessagePrivate);
                await Game.DiscordChannels[GameChannel.TownText].SendMessageAsync($"{p.GotKilled()}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            Game.Client.MessageReactionAdded -= ClientOnMessageReactionAdded;



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

            if (!present || (e.User.GetMember()).Roles.Contains(Game.Roles[CustomRoles.Spectator]))
            {
                await DailyVotingMessage.DeleteReactionAsync(e.Emoji, e.User);
                return;
            }


            if (!e.User.IsBot && !e.User.GetMember().Roles.Contains(Game.Roles[CustomRoles.Spectator]))
            { 
                foreach (var otherEmoji in (await Game.Guild.GetEmojisAsync()))
                {
                    if (otherEmoji.Name != e.Emoji.Name)
                    {
                        await DailyVotingMessage.DeleteReactionAsync(otherEmoji, e.User,
                            $"{e.User.Username} already voted");
                    }
                }
            }
        }

        internal static async Task HunterDeath()
        {
            var hunter = Game.PersonnagesList.Find(p => p.GetType() == typeof(Hunter));
            var message = await hunter.ChannelT.SendMessageAsync(Game.Texts.HunterDeathQuestion);


            foreach (var emoji in (await Game.Guild.GetEmojisAsync()).ToList().FindAll(emo => emo.Id != hunter.Emoji.Id))
            {
                await message.CreateReactionAsync(emoji);
            }

            await Task.Delay(10 * 1000);

            DiscordReaction react = message.Reactions.First(reaction => reaction.Count == message.Reactions.Max(x => x.Count));
            var target = Game.PersonnagesList.Find(p => p.Emoji.Id == react.Emoji.Id);
            await Game.SetSpectatorAsync(target);
            await Game.DiscordChannels[GameChannel.TownText].SendMessageAsync($"{hunter.Me.Username} {Game.Texts.PublicHunterMessage} {target.Me.Username}");
        }

        internal static async Task SeerMoment()
        {
            var seer = Game.PersonnagesList.Find(p => p.GetType() == typeof(Seer));
            var msg = await seer.ChannelT.SendMessageAsync(Game.Texts.SeerAskMsg);

            foreach (var emoji in (await Game.Guild.GetEmojisAsync()).ToList().FindAll(emo => emo.Id != seer.Emoji.Id))
            {
                await msg.CreateReactionAsync(emoji);
            }

            await Task.Delay(10 * 1000);
            DiscordReaction react = msg.Reactions.First(reaction => reaction.Count == msg.Reactions.Max(x => x.Count));
            var target = Game.PersonnagesList.Find(p => p.Emoji.Id == react.Emoji.Id);
            await seer.ChannelT.SendMessageAsync($"{Game.Texts.SeerRecMsg} {target.GetClassName()}");

        }

        internal static async Task WitchMoment()
        {
            throw new NotImplementedException();
        }

        internal static async Task WolfVote()
        {
            throw new NotImplementedException();
        }

        internal static async Task CupidonChoice()
        {
            throw new NotImplementedException();
        }

        public static async Task Elections()
        {
            throw new NotImplementedException();
        }

        public static async Task MakeDeath(Personnage p)
        {
            await Game.SetSpectatorAsync(p);



            if (p.GetType() == typeof(Hunter))
            {
                Game.Moments.Push(Moment.HunterDead);
            }

            if (p.Effect == Effect.Lover)
            {
                var loved = Game.PersonnagesList.Find(p2 => p2.Effect == Effect.Lover && p != p2);
                await Game.DiscordChannels[GameChannel.TownText].SendMessageAsync($"{loved.Me.Username} {Game.Texts.LoveSuicide}");
                await Game.SetSpectatorAsync(loved);
            }

        }
    }
}