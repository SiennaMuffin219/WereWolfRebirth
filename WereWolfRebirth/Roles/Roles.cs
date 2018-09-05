using System;
using DSharpPlus;
using DSharpPlus.Entities;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Env;
using WereWolfRebirth.Env.Extentions;
using WereWolfRebirth.Locale;

namespace WereWolfRebirth.Roles
{
    #region GameRole's Classes

    public class Wolf : Personnage
    {
        public Wolf(Game game, DiscordMember me, DiscordGuildEmoji emoji) : base(game, me, emoji)
        {
            Game.DiscordChannels[GameChannel.WolfText].AddOverwriteAsync(me, GameBuilder.UsrPerms);
            Game.DiscordChannels[GameChannel.WolfVoice].AddOverwriteAsync(me, GameBuilder.UsrPerms);
        }


        public void DoRole()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Game.Texts.WolfToString;
        }

        public override string GotKilled()
        {
            return base.GotKilled() + $"{Game.Texts.WolfName}";
        }

        public override string GetClassName()
        {
            return Game.Texts.WolfName;
        }
    }


    public class Citizen : Personnage
    {
        public Citizen(Game game, DiscordMember me, DiscordGuildEmoji emoji) : base(game, me, emoji)
        {
        }

        public void DoRole()
        {
        }


        public override string ToString()
        {
            return Game.Texts.CitizenToString;
        }

        public override string GotKilled()
        {
            return base.GotKilled() + $"{Game.Texts.CitizenName}";
        }

        public override string GetClassName()
        {
            return Game.Texts.CitizenName;
        }
    }


    public class Salvator : Citizen
    {
        public Salvator(Game game, DiscordMember me, DiscordGuildEmoji emoji) : base(game, me, emoji)
        {
        }

        public new void DoRole()
        {
            throw new NotImplementedException();
        }


        public override string ToString()
        {
            return Game.Texts.SaviorToString + " \n " + Game.Texts.TownFriendly;
        }

        public override string GotKilled()
        {
            return base.GotKilled() + $"{Game.Texts.SaviorName}";
        }

        public override string GetClassName()
        {
            return Game.Texts.SaviorName;
        }
    }


    public class Witch : Citizen
    {
        public Witch(Game game, DiscordMember me, DiscordGuildEmoji emoji) : base(game, me, emoji)
        {
        }


        public override string ToString()
        {
            return Game.Texts.WitchToString + " \n " + Game.Texts.TownFriendly;
        }

        public override string GotKilled()
        {
            return base.GotKilled() + $"{Game.Texts.WitchName}";
        }

        public override string GetClassName()
        {
            return Game.Texts.WitchName;
        }
    }

    public class LittleGirl : Citizen
    {
        public LittleGirl(Game game, DiscordMember me, DiscordGuildEmoji emoji) : base(game, me, emoji)
        {
        }


        public override string ToString()
        {
            return Game.Texts.LittleGirlToString + " \n " + Game.Texts.TownFriendly;
        }

        public override string GotKilled()
        {
            return base.GotKilled() + $"{Game.Texts.LittleGirlName}";
        }

        public override string GetClassName()
        {
            return Game.Texts.LittleGirlName;
        }
    }

    public class Hunter : Citizen
    {
        public Hunter(Game game, DiscordMember me, DiscordGuildEmoji emoji) : base(game, me, emoji)
        {
        }

        public override string ToString()
        {
            return Game.Texts.HunterToString + " \n " + Game.Texts.TownFriendly;
        }

        public override string GotKilled()
        {
            return base.GotKilled() + $"{Game.Texts.HunterName}";
        }

        public override string GetClassName()
        {
            return Game.Texts.HunterName;
        }
    }

    public class Cupidon : Citizen
    {
        public Cupidon(Game game, DiscordMember me, DiscordGuildEmoji emoji) : base(game, me, emoji)
        {
        }

        public override string ToString()
        {
            return Game.Texts.CupidToString + " \n " + Game.Texts.TownFriendly;
        }

        public override string GotKilled()
        {
            return Language.FirstDieMessages(Game, Me) + Game.Texts.CupidName;
        }

        public override string GetClassName()
        {
            return Game.Texts.CupidName;
        }
    }

    public class Seer : Citizen
    {
        public Seer(Game game, DiscordMember me, DiscordGuildEmoji emoji) : base(game, me, emoji)
        {
        }

        public override string ToString()
        {
            return Game.Texts.SeerToString + " \n " + Game.Texts.TownFriendly;
        }

        public override string GotKilled()
        {
            return base.GotKilled() + $"{Game.Texts.SeerName}";
        }

        public override string GetClassName()
        {
            return Game.Texts.SeerName;
        }
    }

    public class TalkativeSeer : Seer
    {
        public TalkativeSeer(Game game, DiscordMember me, DiscordGuildEmoji emoji) : base(game, me, emoji)
        {
        }

        public override string ToString()
        {
            return Game.Texts.SeerToString + " \n " + Game.Texts.TalkativeSeerToString + " \n " +
                   Game.Texts.TownFriendly;
        }

        public override string GotKilled()
        {
            return base.GotKilled() + $"{Game.Texts.TalkativeSeerName}";
        }

        public override string GetClassName()
        {
            return Game.Texts.TalkativeSeerName;
        }
    }

    #endregion

    public abstract class Personnage
    {
        public DiscordMember Me { get; }
        public bool Alive { get; set; }
        public Effect Effect = Effect.None;

        public DiscordChannel ChannelT { get; set; }
        public DiscordChannel ChannelV { get; set; }

        public DiscordGuildEmoji Emoji;

        public Game Game { get; set; }

        protected Personnage(Game game, DiscordMember me, DiscordGuildEmoji emoji)
        {
            Me = me;
            Emoji = emoji;
            Alive = true;
            Game = game;
            var name = Me.Username.RemoveSpecialChars() ?? "jesaispasquoi";


            ChannelV = Game.Guild.CreateChannelAsync(Me.Username.RemoveSpecialChars(), ChannelType.Voice,
                Game.DiscordChannels[GameChannel.PersoGroup]).GetAwaiter().GetResult();
            ChannelT = Game.Guild.CreateChannelAsync(Me.Username.RemoveSpecialChars(), ChannelType.Text,
                Game.DiscordChannels[GameChannel.PersoGroup]).GetAwaiter().GetResult();

            // ReSharper disable once VirtualMemberCallInConstructor


            ChannelT.AddOverwriteAsync(Me, GameBuilder.UsrPerms);
            ChannelV.AddOverwriteAsync(Me, GameBuilder.UsrPerms);

            Game.DiscordChannels[GameChannel.TownText].AddOverwriteAsync(Me, GameBuilder.UsrPerms);
            Game.DiscordChannels[GameChannel.TownVoice].AddOverwriteAsync(Me, GameBuilder.UsrPerms);
            var embed = new DiscordEmbedBuilder
            {
                Title = ToString(),
                Color = Color.InfoColor
            };
            ChannelT.SendMessageAsync(embed: embed.Build()).GetAwaiter().GetResult();
            me.PlaceInAsync(ChannelV);
        }

        public virtual string GotKilled()
        {
            return Me.DisplayName + Game.Texts.DeadMessagePublic;
        }

        public abstract string GetClassName();
    }
}