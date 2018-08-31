﻿using System;
using System.Collections.Generic;
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
            DailyVotingMessage =
                await Game.DiscordChannels[GameChannel.TownText].GetMessageAsync(DailyVotingMessage.Id);
            foreach (var discordReaction in DailyVotingMessage.Reactions)
            {
                Console.WriteLine($"Reaction : {discordReaction.Emoji.Name} : {discordReaction.Count}");
            }

            try
            {
                var emoji = DailyVotingMessage.Reactions
                    .First(x => x.Count == DailyVotingMessage.Reactions.Max(y => y.Count)).Emoji;
                var p = Game.PersonnagesList.Find(personnage => personnage.Emoji.Id == emoji.Id);
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

            var present = false;
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
                        await DailyVotingMessage.DeleteReactionAsync(otherEmoji, e.User, $"{e.User.Username} already voted");
                    }
                }
            }
        }
        private static async Task OnReactionAddedCupidon(MessageReactionAddEventArgs e)
        {

            var present = false;
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

            var cnt = 0;

            if (!e.User.IsBot && !e.User.GetMember().Roles.Contains(Game.Roles[CustomRoles.Spectator]))
            {
                foreach (var otherEmoji in (await Game.Guild.GetEmojisAsync()))
                {
                    cnt++;
                    if (otherEmoji.Name != e.Emoji.Name && cnt > 1)
                    {
                        await DailyVotingMessage.DeleteReactionAsync(otherEmoji, e.User, $"{e.User.Username} already voted");
                        cnt = 0; // 0  car on ajoute 1 avant le test
                    }
                }
            }
        }

        internal static async Task HunterDeath()
        {
            var hunter = Game.PersonnagesList.Find(p => p.GetType() == typeof(Hunter));
            var message = await hunter.ChannelT.SendMessageAsync(Game.Texts.HunterDeathQuestion);


            foreach (var emoji in (await Game.Guild.GetEmojisAsync()).ToList()
                .FindAll(emo => emo.Id != hunter.Emoji.Id))
            {
                await message.CreateReactionAsync(emoji);
            }

            await Task.Delay(10 * 1000);

            var react =
                message.Reactions.First(reaction => reaction.Count == message.Reactions.Max(x => x.Count));
            var target = Game.PersonnagesList.Find(p => p.Emoji.Id == react.Emoji.Id);
            await Game.Kill(target);
            await Game.DiscordChannels[GameChannel.TownText]
                .SendMessageAsync($"{hunter.Me.Username} {Game.Texts.PublicHunterMessage} {target.Me.Username}");
        }

        internal static async Task SeerAction()
        {
            var seer = Game.PersonnagesList.Find(p => p.GetType() == typeof(Seer));
            var msg = await seer.ChannelT.SendMessageAsync(Game.Texts.SeerAskMsg);

            foreach (var emoji in (await Game.Guild.GetEmojisAsync()).ToList().FindAll(emo => emo.Id != seer.Emoji.Id))
            {
                await msg.CreateReactionAsync(emoji);
            }

            await Task.Delay(10 * 1000);
            var react = msg.Reactions.First(reaction => reaction.Count == msg.Reactions.Max(x => x.Count));
            var target = Game.PersonnagesList.Find(p => p.Emoji.Id == react.Emoji.Id);
            await seer.ChannelT.SendMessageAsync($"{Game.Texts.SeerRecMsg} {target.GetClassName()}");
        }

        internal static async Task WitchMoment()
        {
            var witch = Game.PersonnagesList.Find(p => p.GetType() == typeof(Witch));
            var witchCh = witch.ChannelT;

            var healMsg = await witchCh.SendMessageAsync($"{Game.NightTargets[0]} {Game.Texts.WitchSaveMsg}");
            await healMsg.CreateReactionAsync(DiscordEmoji.FromName(Game.Client, ":thumbsup:"));
            await healMsg.CreateReactionAsync(DiscordEmoji.FromName(Game.Client, ":thumbsdown:"));

            Game.Client.MessageReactionAdded += ClientOnMessageReactionAdded;

            await Task.Delay(5_000);
            healMsg = await witchCh.GetMessageAsync(healMsg.Id);
            if (healMsg.GetReactionsAsync(DiscordEmoji.FromName(Game.Client, ":thumbsup:")).GetAwaiter().GetResult().Count == 2)
            {
                Game.NightTargets.Clear();
            }


            var killMsg = await witchCh.SendMessageAsync($"{Game.Texts.WitchKillMsg}");
            foreach (var emoji in Game.Guild.GetEmojisAsync().GetAwaiter().GetResult().ToList().FindAll(e => e.Id != witch.Emoji.Id))
            {
                await killMsg.CreateReactionAsync(emoji);
            }

            Game.Client.MessageReactionAdded += ClientOnMessageReactionAdded;

            await Task.Delay(5_000);
            killMsg = await witchCh.GetMessageAsync(killMsg.Id);
            Game.NightTargets.Add(Game.PersonnagesList.Find(p => p.Emoji == killMsg.Reactions.ToList().Find(r => r.Count == 2).Emoji));
        }

        internal static async Task WolfVote()
        {
            Game.NightTargets = new List<Personnage>();

            var msg = await Game.DiscordChannels[GameChannel.WolfText].SendMessageAsync(Game.Texts.NightlyWolfMessage);

            foreach (var personnage in Game.PersonnagesList.FindAll(p => p.GetType() != typeof(Wolf) && p.Alive))
            {
                await msg.CreateReactionAsync(personnage.Emoji);
            }

            await Task.Delay(10_000);
            msg = await Game.DiscordChannels[GameChannel.WolfText].GetMessageAsync(msg.Id);
            var react = msg.Reactions.First(reaction => reaction.Count == msg.Reactions.Max(x => x.Count));
            var target = Game.PersonnagesList.Find(p => p.Emoji.Id == react.Emoji.Id);

            Game.NightTargets.Add(target);



        }


        internal static async Task CupidonChoice()
        {
            var channel = Game.PersonnagesList.Find(p => p.GetType() == typeof(Cupidon)).ChannelT;
            var msg = await channel.SendMessageAsync(Game.Texts.CupidMessage);
            Game.Client.MessageReactionAdded += OnReactionAddedCupidon;

            await Task.Delay(10_000);
            
            msg = await channel.GetMessageAsync(msg.Id);

            var react = msg.Reactions.ToList().FindAll(reaction => reaction.Count == msg.Reactions.Max(x => x.Count));

            var target = new Personnage[]
            {
                Game.PersonnagesList.Find(p => p.Emoji.Id == react[0].Emoji.Id),
                Game.PersonnagesList.Find(p => p.Emoji.Id == react[1].Emoji.Id)

            };

            foreach (var personnage in target)
            {
                personnage.Effect = Effect.Lover;
            }

            Game.Client.MessageReactionAdded -= OnReactionAddedCupidon;

        }


        public static async Task EndNight()
        {
            foreach (var target in Game.NightTargets)
            {
                await target.ChannelT.SendMessageAsync($"{Game.Texts.DeadMessagePrivate}");
                await Game.Kill(target);
            }
        }

        public static async Task Elections()
        {
            throw new NotImplementedException();
        }

        public static async Task MakeDeath(Personnage p)
        {
            await Game.Kill(p);


            if (p.GetType() == typeof(Hunter))
            {
                Game.Moments.Push(Moment.HunterDead);
            }

            if (p.Effect == Effect.Lover)
            {
                var loved = Game.PersonnagesList.Find(p2 => p2.Effect == Effect.Lover && p != p2);
                await Game.DiscordChannels[GameChannel.TownText]
                    .SendMessageAsync($"{loved.Me.Username} {Game.Texts.LoveSuicide}");
                await Game.Kill(loved);
            }
        }

        public static Task LittleGirlAction()
        {
            throw new NotImplementedException();
        }
    }
}