using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BotDiscord.Env.Enum;
using BotDiscord.Env.Extentions;
using BotDiscord.Roles;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Drawing;
namespace BotDiscord.Env
{

    public static class Global
    {
        public static List<Game> Games = new List<Game>();
        public static Dictionary<CustomRoles, DiscordRole> Roles { get; set; }

        public static int currGame = 0;
        public static DiscordClient Client { get; set; }
    }


    public static class GameBuilder
    {
        public static Permissions UsrPerms = CreatePerms(Permissions.AccessChannels, Permissions.AddReactions,
            Permissions.SendMessages);

        public static async Task CreatePersonnages(Game game, List<DiscordMember> players)
        {
            try
            {
                var roles = CreateRoles(players.Count);
                var rand = new Random(DateTime.Now.Millisecond);


                game.PersonnagesList = new List<Personnage>();

                var letter = 'a';

                while (players.Count != 0)
                {
                    var nbRand = rand.Next(roles.Count);
                    DiscordGuildEmoji emoji;
                    try
                    {
                        //Image image;


                        // Save image to stream.
                        Stream stream2;
                        var name = players[0].Username.RemoveSpecialChars();

                        Console.WriteLine(name + " : " + players[0].AvatarUrl);

                        if (players[0].AvatarUrl == players[0].DefaultAvatarUrl)
                        {
                            // image = Image.FromFile();
                            stream2 = new FileStream($"..//..//Images//UserIcons//{letter}.png", FileMode.Open);
                            letter = (char) (Convert.ToUInt32(letter) + 1);
                        }
                        else
                        {
                            new WebClient().DownloadFile(players[0].AvatarUrl.Replace("size=1024", "size=256"),
                                $"..//..//Images//UserIcons//{name}.png");
                            //image = Image.FromFile($"..//..//Images//UserIcons//{name}.png");
                            stream2 = new FileStream($"..//..//Images//UserIcons//{name}.png", FileMode.Open);
                        }

                        //image.Save(stream, ImageFormat.Png);

                        emoji = await game.Guild.CreateEmojiAsync(name, stream2);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Erreur Emoji");
                        Console.WriteLine(e);
                        emoji = DiscordEmoji.FromName(Global.Client, ":yum:") as DiscordGuildEmoji;
                    }


                    switch (roles[nbRand])
                    {
                        case GameRole.Citizen:
                            game.PersonnagesList.Add(new Citizen(game, players[0], emoji));
                            break;
                        case GameRole.Hunter:
                            game.PersonnagesList.Add(new Hunter(game, players[0], emoji));
                            break;
                        case GameRole.Cupid:
                            game.PersonnagesList.Add(new Cupidon(game, players[0], emoji));
                            break;
                        case GameRole.Witch:
                            game.PersonnagesList.Add(new Witch(game, players[0], emoji));
                            break;
                        case GameRole.Savior:
                            game.PersonnagesList.Add(new Salvator(game, players[0], emoji));
                            break;
                        case GameRole.Seer:
                            game.PersonnagesList.Add(new Seer(game, players[0], emoji));
                            break;
                        case GameRole.TalkativeSeer:
                            game.PersonnagesList.Add(new TalkativeSeer(game, players[0], emoji));
                            break;
                        case GameRole.LittleGirl:
                            game.PersonnagesList.Add(new LittleGirl(game, players[0], emoji));
                            break;

                        case GameRole.Wolf:
                            game.PersonnagesList.Add(new Wolf(game, players[0], emoji));
                            break;
                    }

                    roles.RemoveAt(nbRand);
                    players.RemoveAt(0);
                }

                foreach (var dm in players)
                {
                    await game.DiscordChannels[GameChannel.BotVoice].AddOverwriteAsync(dm, Permissions.None, Permissions.AccessChannels);
                    await game.DiscordChannels[GameChannel.BotText].AddOverwriteAsync(dm, Permissions.None, Permissions.AccessChannels);
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

        public static void Debug(Game game)
        {
            if (game.PersonnagesList is null)
            {
                Console.WriteLine("Il n'y a aucun personnage joueur dans le jeu");
            }
            else
            {
                var i = 0;
                foreach (var p in game.PersonnagesList)
                {
                    Console.WriteLine(i + " : " + p);
                    i++;
                }
            }
        }

        public static async Task CreateDiscordRoles(Game game)
        {
            #region Roles

            Global.Roles = new Dictionary<CustomRoles, DiscordRole>();


            var adminRole = await game.Guild.CreateRoleAsync(game.Texts.BotName, Permissions.Administrator, Color.AdminColor, true, true, "GameRole Bot");
            Global.Roles.Add(CustomRoles.Admin, adminRole);


            var playerPerms = GameBuilder.CreatePerms(Permissions.SendMessages, Permissions.ReadMessageHistory,
                Permissions.AddReactions);

            var playerRole = await game.Guild.CreateRoleAsync(game.Texts.Player, playerPerms, Color.PlayerColor, true, true, "GameRole Joueur");
            Global.Roles.Add(CustomRoles.Player, playerRole);


            var spectPerms = GameBuilder.CreatePerms(Permissions.AccessChannels, Permissions.ReadMessageHistory);
            GameBuilder.RevokePerm(spectPerms, Permissions.ManageEmojis);
            var spectRole = await game.Guild.CreateRoleAsync(game.Texts.Spectator, spectPerms, Color.SpectColor, true, false, "GameRole spectateur");

            Global.Roles.Add(CustomRoles.Spectator, spectRole);


            await game.Guild.EveryoneRole.ModifyAsync(x => x.Permissions = Permissions.None);

            #endregion
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

        public static class DiscordUserExtention
        {
        }

        public static DiscordMember GetMember(DiscordGuild Guild, DiscordUser usr) => Guild.GetMemberAsync(usr.Id).GetAwaiter().GetResult();


    }
}