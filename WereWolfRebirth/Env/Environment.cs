using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Env.Extentions;
using WereWolfRebirth.Locale;
using WereWolfRebirth.Roles;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace WereWolfRebirth.Env
{
    internal static class Game
    {
        public static Dictionary<GameChannel, DiscordChannel> DiscordChannels;
        public static Language Texts;
        public static bool Wait = true;
        public static List<Personnage> PersonnagesList;
        public static Victory Victory = Victory.None;
        public static DiscordClient Client = null;
        public static ulong GuildId;
        public static DiscordGuild Guild = null;
        public static Dictionary<CustomRoles, DiscordRole> Roles { get; set; }
        public static Stack<Moment> Moments;
        public static List<Personnage> NightTargets;
        public static Task Play;

        public static int Laps = 0;

        public static void SetLanguage(string lang)
        {
            // ReSharper disable once PossibleNullReferenceException
            Texts = JsonConvert.DeserializeObject<Language>(File.ReadAllText(
                $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}/Locale/{lang}/lang.json",
                Encoding.UTF8));
        }


        public static Permissions CreatePerms(params Permissions[] perms)
        {
            return GrantPerm(Permissions.None, perms);
        }


        public static Permissions GrantPerm(Permissions p, params Permissions[] grant)
        {
            foreach (var pg in grant)
            {
                p |= pg;
            }

            return p;
        }

        public static Permissions RevokePerm(Permissions p, params Permissions[] grant)
        {
            foreach (var pg in grant)
            {
                p &= ~pg;
            }

            return p;
        }

        public static void CheckVictory()
        {
            // Si il n'y a pas de loup = la ville gagne 
            var nbWolves = PersonnagesList.FindAll(p => p.GetType() == typeof(Wolf)).Count;
            if (nbWolves == 0)
            {
                Victory = Victory.Town;
            }

            // Si il n'y a que des loups = les loups gagne 
            if (nbWolves == PersonnagesList.FindAll(p => p.Alive).Count)
            {
                Victory = Victory.Wolf;
            }

            // On check si les amoureux sont les seuls restant 
            if (PersonnagesList.FindAll(p => p.Effect == Effect.Lover && p.Alive).Count ==
                PersonnagesList.FindAll(p2 => p2.Alive).Count)
            {
                Victory = Victory.Lovers;
            }
        }


        public static void WriteDebug(object o)
        {
            Console.WriteLine("DEBUG\t" + o);
        }

        public static async Task PlayAsync()
        {


            try
            {
                Laps++;

                CreateStack();

                WriteDebug($"Laps : {Laps}");

                while (Moments.Count > 0)
                {
                    WriteDebug($"Moment Active : {Moments.Peek()}");

                    foreach (var moment in Moments.ToArray())
                    {
                        WriteDebug($"Moment in pile : {moment}");

                    }


                    switch (Moments.Pop())
                    {
                        case Moment.Voting:
                            await BotFunctions.DailyVote();
                            break;

                        case Moment.HunterDead:
                            await BotFunctions.HunterDeath();
                            break;

                        case Moment.EndNight:
                            await BotFunctions.EndNight();
                            break;

                        case Moment.NightPhase1:
                            await BotFunctions.WolfVote();
                            await BotFunctions.SeerAction();
                            await BotFunctions.LittleGirlAction();
                            break;

                        case Moment.NightPhase2:
                            await BotFunctions.WitchMoment();
                            break;

                        case Moment.Election:
                            await BotFunctions.Elections();
                            break;

                        case Moment.Cupid:
                            await BotFunctions.CupidonChoice();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                CheckVictory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Ending()
        {

        }

        public static void CreateStack()
        {
            Moments = new Stack<Moment>();


            Moments.Push(Moment.Voting);
            Moments.Push(Moment.EndNight); // Tue vraiment les targets 
            if (Game.PersonnagesList.FindAll(p => p.GetType() == typeof(Witch)).Count >= 1)
            {
                Moments.Push(Moment.NightPhase2); // Witch 
            }

            Moments.Push(Moment.NightPhase1); // lg, pf, voyante 


            if (Laps == 1 && PersonnagesList.FindAll(p => p.GetType() == typeof(Cupidon)).Count >= 1)
            {
                Moments.Push(Moment.Cupid);
            }

            Laps++;
        }


        public static async Task Kill(Personnage p)
        {
            try
            {
                p.Alive = false;
                await DiscordChannels[GameChannel.TownText].SendMessageAsync($"{p.Me.Username} {Texts.DeadMessagePublic}");


                foreach (var discordChannel in DiscordChannels.Values)
                {
                    await discordChannel.AddOverwriteAsync(p.Me, Permissions.AccessChannels);
                }

                await p.Me.RevokeRoleAsync(Roles[CustomRoles.Player]);
                await p.Me.GrantRoleAsync(Roles[CustomRoles.Spectator]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public static class GameBuilder
    {
        public static Permissions UsrPerms = Game.CreatePerms(Permissions.AccessChannels, Permissions.AddReactions,
            Permissions.SendMessages);

        public static async Task CreatePersonnages(List<DiscordMember> players)
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

                            var name = players[0].Username.RemoveSpecialChars();

                            Console.WriteLine(name + " : " + players[0].AvatarUrl);

                            if (players[0].AvatarUrl == players[0].DefaultAvatarUrl)
                            {
                                image = Image.FromFile($"..//..//Images//UserIcons//{letter}.png");
                                letter = (char) (Convert.ToUInt32(letter) + 1);
                            }
                            else
                            {
                                new WebClient().DownloadFile(players[0].AvatarUrl.Replace("size=1024", "size=256"),
                                    $"..//..//Images//UserIcons//{name}.png");
                                image = Image.FromFile($"..//..//Images//UserIcons//{name}.png");
                            }

                            image.Save(stream, ImageFormat.Png);

                            emoji = await Game.Guild.CreateEmojiAsync(name, stream);
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
                        case GameRole.Citizen:
                            Game.PersonnagesList.Add(new Citizen(players[0], emoji));
                            break;
                        case GameRole.Hunter:
                            Game.PersonnagesList.Add(new Hunter(players[0], emoji));
                            break;
                        case GameRole.Cupid:
                            Game.PersonnagesList.Add(new Cupidon(players[0], emoji));
                            break;
                        case GameRole.Witch:
                            Game.PersonnagesList.Add(new Witch(players[0], emoji));
                            break;
                        case GameRole.Savior:
                            Game.PersonnagesList.Add(new Salvator(players[0], emoji));
                            break;
                        case GameRole.Seer:
                            Game.PersonnagesList.Add(new Seer(players[0], emoji));
                            break;
                        case GameRole.TalkativeSeer:
                            Game.PersonnagesList.Add(new TalkativeSeer(players[0], emoji));
                            break;
                        case GameRole.LittleGirl:
                            Game.PersonnagesList.Add(new LittleGirl(players[0], emoji));
                            break;

                        case GameRole.Wolf:
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


        public static List<GameRole> CreateRoles(int nbPlayer)
        {
            var roleList = new List<GameRole>();

            for (var i = 0; i < nbPlayer; i++)
            {
                switch (i)
                {
                    case 1:
                        roleList.Add(GameRole.Citizen);
                        break;
                    case 2:
                        roleList.Add(GameRole.Wolf);
                        break;
                    case 3:
                        roleList.Add(GameRole.Seer);
                        break;
                    case 4:
                        roleList.Add(GameRole.Wolf);
                        break;
                    case 5:
                        roleList.Add(GameRole.Savior);
                        roleList.Add(GameRole.Citizen);
                        roleList.Add(GameRole.Wolf);
                        break;
                    case 6:
                        roleList.Add(GameRole.LittleGirl);
                        break;
                    case 7:
                        roleList.Add(GameRole.Witch);
                        break;
                    case 8:
                        roleList.Add(GameRole.Hunter);
                        break;
                    case 9:
                        roleList.Add(GameRole.Wolf);
                        break;
                    case 10:
                        roleList.Add(GameRole.Cupid);
                        break;
                    default:
                        roleList.Add(i % 3 == 0 ? GameRole.Wolf : GameRole.Citizen);
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

        public static async Task CreateDiscordRoles()
        {
            #region Roles

            Game.Roles = new Dictionary<CustomRoles, DiscordRole>();


            var adminRole = await Game.Guild.CreateRoleAsync(Game.Texts.BotName, Permissions.Administrator,
                new DiscordColor("#EE0000"), true, true, "GameRole Bot");
            Game.Roles.Add(CustomRoles.Admin, adminRole);


            var playerPerms = Game.CreatePerms(Permissions.SendMessages, Permissions.ReadMessageHistory,
                Permissions.AddReactions);

            var playerRole = await Game.Guild.CreateRoleAsync(Game.Texts.Player, playerPerms,
                new DiscordColor("#1de020"), true, true, "GameRole Joueur");
            Game.Roles.Add(CustomRoles.Player, playerRole);


            var spectPerms = Game.CreatePerms(Permissions.AccessChannels, Permissions.ReadMessageHistory);
            Game.RevokePerm(spectPerms, Permissions.ManageEmojis);
            var spectRole = await Game.Guild.CreateRoleAsync(Game.Texts.Spectator, spectPerms,
                new DiscordColor("#7200a3"), true, false, "GameRole spectateur");

            Game.Roles.Add(CustomRoles.Spectator, spectRole);


            await Game.Guild.EveryoneRole.ModifyAsync(x => x.Permissions = Permissions.None);

            #endregion
        }
    }
}