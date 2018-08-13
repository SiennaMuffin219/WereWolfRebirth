using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Env;

namespace WereWolfRebirth.Roles
{
    static class GameBuilder
    {
        public static async Task CreatePersonnages(List<DiscordUser> players)
        {
            try
            {


                List<Role> roles = CreateRoles(players.Count);
                Random rand = new Random(DateTime.Now.Millisecond);


                Game.PersonnagesList = new List<Personnage>();

                char letter = 'a';

                while (players.Count != 0)
                {
                    int nbRand = rand.Next(roles.Count);
                    DiscordGuildEmoji emoji = null;

                    Stream img = null;

                    if (players[0].AvatarUrl == players[0].DefaultAvatarUrl)
                    {
                        img = File.Open($"../..//Images//UserIcons//{letter}.png", FileMode.Open);
                        letter = (char) (Convert.ToUInt32(letter) + 1);
                    }
                    else
                    {
                        img = new System.Net.WebClient().OpenRead(players[0].AvatarUrl);
                        emoji = await Game.guild.CreateEmojiAsync(players[0].Username, img);

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
            catch (System.Exception ex1)
            {

                System.Console.WriteLine(ex1);
            }


        }


        static public List<Role> CreateRoles(int NbPlayer)
        {
            List<Role> RoleList = new List<Role>();

            for (int i = 0; i < NbPlayer; i++)
            {
                switch (i)
                {
                    case 1:
                        RoleList.Add(Role.Citizien); break;
                    case 2:
                        RoleList.Add(Role.Wolf); break;
                    case 3:
                        RoleList.Add(Role.Seer); break;
                    case 4:
                        RoleList.Add(Role.Wolf); break;
                    case 5:
                        RoleList.Add(Role.Savior);
                        RoleList.Add(Role.Citizien);
                        RoleList.Add(Role.Wolf); break;
                    case 6:
                        RoleList.Add(Role.LittleGirl); break;
                    case 7:
                        RoleList.Add(Role.Witch); break;
                    case 8:
                        RoleList.Add(Role.Hunter); break;
                    case 9:
                        RoleList.Add(Role.Wolf); break;
                    case 10:
                        RoleList.Add(Role.Cupid); break;
                    default:
                        RoleList.Add(i % 3 == 0 ? Role.Wolf : Role.Citizien); break;
                }
            }
            return RoleList;
        }

        static public void Debug()
        {
            if (Game.PersonnagesList is null)
            {
                Console.WriteLine("Il n'y a aucun personnage joueur dans le jeu");
            }
            else
            {

                int i = 0;
                foreach (Personnage p in Game.PersonnagesList)
                {
                    Console.WriteLine(i + " : " + p.ToString());
                    i++;
                }

            }
        }

    }


    #region Role's Classes


    class Wolf : Personnage
    {
        public Wolf(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }

        public void doRole()
        {
            throw new NotImplementedException();
        }


        public static bool HasWon()
        {
            return !Game.PersonnagesList.Exists(x =>
                x.GetType() == Type.GetType("WereWolfRebirth.Roles.Citizien") ||
                x.GetType() == Type.GetType("WereWolfRebirth.Roles.PiedPiper") ||
                x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Citizien")));

        }


        public override string ToString()
        {
            return Game.TextJson.WolfToString;
        }

    }


    class Citizien : Personnage
    {


        public Citizien(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }

        public void doRole()
        {

        }

        public static bool HasWon()
        {
            return !Game.PersonnagesList.Exists(x =>
                x.GetType() == Type.GetType("WereWolfRebirth.Roles.Wolf") ||
                x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Wolf")));
        }

        public override string ToString()
        {
            return Game.TextJson.CitizienToString;
        }

    }



    class Salvator : Citizien
    {
        public Salvator(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        {
        }

        public new void doRole()
        {
            throw new NotImplementedException();
        }

        public static new bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.SaviorToString + " \n " + Game.TextJson.TownFriendly;
        }



    }


    class Witch : Citizien
    {
        public Witch(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public static new bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.WitchToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    class LittleGirl : Citizien
    {
        public LittleGirl(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public static new bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.LittleGirlToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    class Hunter : Citizien
    {
        public Hunter(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public static new bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.HunterToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    class Cupidon : Citizien
    {
        public Cupidon(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public static new bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.CupidToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    class Seer : Citizien
    {
        public Seer(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public static new bool HasWon() => Citizien.HasWon();

        public override string ToString()
        {
            return Game.TextJson.SeerToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    class TalkativeSeer : Seer
    {
        public TalkativeSeer(DiscordUser me, DiscordGuildEmoji emoji) : base(me, emoji)
        { }
        public static new bool HasWon() => Seer.HasWon();

        public override string ToString()
        {
            return Game.TextJson.SeerToString + " \n " + Game.TextJson.TalkativeSeerToString + " \n " + Game.TextJson.TownFriendly;
        }
    }

    #endregion

    public abstract class Personnage
    {

        public DiscordUser Me { get; private set; }
        public bool Alive {get; set;}
        public Effect bonus = Effect.None;

        public DiscordGuildEmoji Emoji = null;

        public Personnage(DiscordUser me, DiscordGuildEmoji emoji)
        {
            Me = me;
            Emoji = emoji;
        }



    }



}