using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Env;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace WereWolfRebirth.Roles
{
    public static class GameBuilder
    {
        public static Permissions UsrPerms = Game.CreatePerms(Permissions.AccessChannels, Permissions.AddReactions, Permissions.SendMessages);

        public static async Task CreatePersonnages(List<DiscordUser> players)
        {
            try
            {

                var roles = CreateRoles(players.Count);
                var rand = new Random(DateTime.Now.Millisecond);


                Game.PersonnagesList = new List<Personnage>();

                var letter = 'a';

                while (players.Count != 0)
                {
                    var nbRand = rand.Next(roles.Count);
                    DiscordGuildEmoji emoji;
                    try
                    {



                        Image image;
                        using (var stream = new MemoryStream())
                        {
                            // Save image to stream.

                             

                            Console.WriteLine(players[0].Username + " : " + players[0].AvatarUrl);

                            if (players[0].AvatarUrl == players[0].DefaultAvatarUrl)
                            {
                                image = Image.FromFile($"..//..//Images//UserIcons//{letter}.png");
                                letter = (char) (Convert.ToUInt32(letter) + 1);
                            }
                            else
                            {
                              
                                new WebClient().DownloadFile(players[0].AvatarUrl.Replace("size=1024", "size=256"),
                                    $"..//..//Images//UserIcons//{WebUtility.HtmlEncode(players[0].Username)}.png");
                                image = Image.FromFile($"..//..//Images//UserIcons//{WebUtility.HtmlEncode(players[0].Username)}.png");
                            }

                            image.Save(stream, ImageFormat.Png);

                            emoji = await Game.guild.CreateEmojiAsync(WebUtility.HtmlEncode(players[0].Username), stream);

                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Erreur Emoji");
                        Console.WriteLine(e);
                        emoji = DiscordEmoji.FromName(Game.Client, ":yum:") as DiscordGuildEmoji;
                    }


                    switch (roles[nbRand])
                    {
                        case Role.Citizien:
                            Game.PersonnagesList.Add(new Citizien(players[0], emoji));
                            break;
                        case Role.Hunter:
                            Game.PersonnagesList.Add(new Hunter(players[0], emoji));
                            break;
                        case Role.Cupid:
                            Game.PersonnagesList.Add(new Cupidon(players[0], emoji));
                            break;
                        case Role.Witch:
                            Game.PersonnagesList.Add(new Witch(players[0], emoji));
                            break;
                        case Role.Savior:
                            Game.PersonnagesList.Add(new Salvator(players[0], emoji));
                            break;
                        case Role.Seer:
                            Game.PersonnagesList.Add(new Seer(players[0], emoji));
                            break;
                        case Role.TalkativeSeer:
                            Game.PersonnagesList.Add(new TalkativeSeer(players[0], emoji));
                            break;
                        case Role.LittleGirl:
                            Game.PersonnagesList.Add(new LittleGirl(players[0], emoji));
                            break;

                        case Role.Wolf:
                            Game.PersonnagesList.Add(new Wolf(players[0], emoji));
                            break;

                    }

                    roles.RemoveAt(nbRand);
                    players.RemoveAt(0);

                }
            }
            catch (Exception ex1)
            {

                Console.WriteLine(ex1);
            }


        }


        public static List<Role> CreateRoles(int nbPlayer)
        {
            var roleList = new List<Role>();

            for (var i = 0; i < nbPlayer; i++)
            {
                switch (i)
                {
                    case 1:
                        roleList.Add(Role.Citizien);
                        break;
                    case 2:
                        roleList.Add(Role.Wolf);
                        break;
                    case 3:
                        roleList.Add(Role.Seer);
                        break;
                    case 4:
                        roleList.Add(Role.Wolf);
                        break;
                    case 5:
                        roleList.Add(Role.Savior);
                        roleList.Add(Role.Citizien);
                        roleList.Add(Role.Wolf);
                        break;
                    case 6:
                        roleList.Add(Role.LittleGirl);
                        break;
                    case 7:
                        roleList.Add(Role.Witch);
                        break;
                    case 8:
                        roleList.Add(Role.Hunter);
                        break;
                    case 9:
                        roleList.Add(Role.Wolf);
                        break;
                    case 10:
                        roleList.Add(Role.Cupid);
                        break;
                    default:
                        roleList.Add(i % 3 == 0 ? Role.Wolf : Role.Citizien);
                        break;
                }
            }

            return roleList;
        }

        public static void Debug()
        {
            if (Game.PersonnagesList is null)
            {
                Console.WriteLine("Il n'y a aucun personnage joueur dans le jeu");
            }
            else
            {

                var i = 0;
                foreach (var p in Game.PersonnagesList)
                {
                    Console.WriteLine(i + " : " + p);
                    i++;
                }

            }
        }


    }

    #region Role's Classes

        public class Wolf : Personnage
        {
            public Wolf(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
            {
                Game.DiscordChannels[GameChannel.WolfText].AddOverwriteAsync(me as DiscordMember, GameBuilder.UsrPerms);
                Game.DiscordChannels[GameChannel.WolfVoice].AddOverwriteAsync(me as DiscordMember, GameBuilder.UsrPerms);
            }


        public void DoRole()
            {
                throw new NotImplementedException();
            }


            public static bool HasWon()
            {
                return !Game.PersonnagesList.Exists(x =>
                    x.GetType() == Type.GetType("WereWolfRebirth.Roles.Citizien") ||
                    x.GetType() == Type.GetType("WereWolfRebirth.Roles.PiedPiper") ||
                    x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Citizien") ?? throw new InvalidOperationException()));

            }


            public override string ToString()
            {
                return Game.TextJson.WolfToString;
            }

    }


    public class Citizien : Personnage
    {


        public Citizien(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }

        public void DoRole()
        {

        }

        public static bool HasWon()
        {
            return !Game.PersonnagesList.Exists(x =>
                x.GetType() == Type.GetType("WereWolfRebirth.Roles.Wolf") ||
                x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Wolf") ?? throw new InvalidOperationException()));
        }

        public override string ToString()
        {
            return Game.TextJson.CitizienToString;
        }

    }


    public class Salvator : Citizien
    {
        public Salvator(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        {
        }

        public new void DoRole()
        {
            throw new NotImplementedException();
        }

        public new static bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.SaviorToString + " \n " + Game.TextJson.TownFriendly;
        }



    }


    public class Witch : Citizien
    {
        public Witch(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public new static bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.WitchToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    public class LittleGirl : Citizien
    {
        public LittleGirl(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public new static bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.LittleGirlToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    public class Hunter : Citizien
    {
        public Hunter(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public new static bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.HunterToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    public class Cupidon : Citizien
    {
        public Cupidon(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public new static bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.CupidToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    public class Seer : Citizien
    {
        public Seer(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public new static bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.SeerToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    public class TalkativeSeer : Seer
    {
        public TalkativeSeer(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public new static bool HasWon() => Seer.HasWon();

        public override string ToString()
        {
            return Game.TextJson.SeerToString + " \n " + Game.TextJson.TalkativeSeerToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    #endregion

    public class Personnage
    {

        public DiscordUser Me { get; private set; }
        public bool Alive {get; set;}
        public Effect Bonus = Effect.None;

        public DiscordChannel ChannelT { get; set; }
        public DiscordChannel ChannelV { get; set; }

        public DiscordGuildEmoji Emoji;

        public Personnage(DiscordUser me, DiscordGuildEmoji emoji)
        {
            Me = me;
            Emoji = emoji;
            Alive = true;

            ChannelV = Game.guild.CreateChannelAsync(Me.Username, ChannelType.Voice, Game.DiscordChannels[GameChannel.PersoGroup]).GetAwaiter().GetResult();
            ChannelT = Game.guild.CreateChannelAsync(Me.Username, ChannelType.Text, Game.DiscordChannels[GameChannel.PersoGroup]).GetAwaiter().GetResult();

            // ReSharper disable once VirtualMemberCallInConstructor
            ChannelT.SendMessageAsync(ToString()).GetAwaiter().GetResult();


            ChannelT.AddOverwriteAsync(me as DiscordMember, GameBuilder.UsrPerms);
            ChannelV.AddOverwriteAsync(me as DiscordMember, GameBuilder.UsrPerms);

            Game.DiscordChannels[GameChannel.TownText].AddOverwriteAsync(me as DiscordMember, GameBuilder.UsrPerms);
            Game.DiscordChannels[GameChannel.TownVoice].AddOverwriteAsync(me as DiscordMember, GameBuilder.UsrPerms);

        }



    }



}