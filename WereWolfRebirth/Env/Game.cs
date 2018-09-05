using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Locale;
using WereWolfRebirth.Roles;

namespace WereWolfRebirth.Env
{
    public class Game
    {
        public Dictionary<GameChannel, DiscordChannel> DiscordChannels;
        public Language Texts;
        public bool Wait = true;
        public List<Personnage> PersonnagesList;
        public Victory Victory = Victory.None;
        public ulong GuildId;
        public DiscordGuild Guild;
        public Stack<Moment> Moments { get; set; }
        public List<Personnage> NightTargets { get; set; }

        public int Laps;

        public int Id;
        public static int Ids;

        public Game(CommandContext e, string lang)
        {
            Id = Ids;
            Ids++;
            WriteDebug($"{e.User.Username}\t{lang}");
            //CreateGuild(e, lang).GetAwaiter().GetResult();
            Global.currGame = Id;
        }


        public void SetLanguage(string lang)
        {
            // ReSharper disable once PossibleNullReferenceException
            Texts = JsonConvert.DeserializeObject<Language>(File.ReadAllText(
                $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}/Locale/{lang}/lang.json",
                Encoding.UTF8));
        }





        public async Task CreateGuild(CommandContext e, string lang = "fr")
        {
            Global.Client = e.Client;
            SetLanguage(lang);
            try
            {
                var msgs = (await e.Guild.GetDefaultChannel().GetMessagesAsync(10)).ToList()
                    .FindAll(m => m.Author == e.Client.CurrentUser || m.Content.Contains("!go"));
                if (msgs.Count > 0)
                {
                    await e.Guild.GetDefaultChannel().DeleteMessagesAsync(msgs);

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }


            while (Guild == null)
                try
                {
                    Guild = e.Client.CreateGuildAsync("Loup Garou").GetAwaiter().GetResult();
                    GuildId = Guild.Id;
                    await Guild.ModifyAsync(x => x.SystemChannel = null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            await GameBuilder.CreateDiscordRoles(this);

            await GameBuilder.GetMember(Guild, Global.Client.CurrentUser).GrantRoleAsync(Global.Roles[CustomRoles.Admin]);

            await (await Guild.GetAllMembersAsync()).First().ModifyAsync(m => m.Nickname = Texts.BotName);

            Console.WriteLine("Guild Created");

            DiscordChannels = new Dictionary<GameChannel, DiscordChannel>();

            Console.WriteLine("Delatation faite");

            await e.TriggerTypingAsync();

            var generalChannel = Guild.GetDefaultChannel();
            await generalChannel.ModifyAsync(x => x.Name = "Bot");
            DiscordChannels.Add(GameChannel.BotText, generalChannel);

            var botVChannel = await Guild.CreateChannelAsync("Bot", ChannelType.Voice, generalChannel.Parent);
            DiscordChannels.Add(GameChannel.BotVoice, botVChannel);
            e.Client.GuildMemberAdded += NewGuildMember;
            e.Client.GuildMemberAdded += StartMember;


            var inv = await generalChannel.CreateInviteAsync();

            var msgInv = await e.RespondAsync(inv.ToString());

            var embed = new DiscordEmbedBuilder
            {
                Title = Texts.BotWantPlay,
                Color = Color.PollColor
            };
            var askMessage = await generalChannel.SendMessageAsync(embed: embed.Build());
            var emoji = DiscordEmoji.FromName(e.Client, ":thumbsup:");
            await askMessage.CreateReactionAsync(emoji);


            var players = new List<DiscordMember>();


            try
            {
                var timeToJoin = 10;
                await Task.Delay(timeToJoin * 1000);

                var users = await (await Guild.GetDefaultChannel().GetMessageAsync(askMessage.Id))
                    .GetReactionsAsync(emoji);

                foreach (var usr in users)
                    if (!usr.IsBot)
                    {
                        var dm = await Guild.GetMemberAsync(usr.Id);
                        await dm.RevokeRoleAsync(Global.Roles[CustomRoles.Spectator]);
                        await dm.GrantRoleAsync(Global.Roles[CustomRoles.Player]);
                        players.Add(dm);
                    }

                // DEBUG
                foreach (var discordMember in players) WriteDebug($"Il y a {discordMember.Username} dans le jeu");

                e.Client.GuildMemberAdded -= StartMember;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            Console.WriteLine(13);
            try
            {
                GameBuilder.Debug(this);
                var channelsToRemove = new List<DiscordChannel>();
                while (Guild.Channels.Count != DiscordChannels.Count)
                {
                    foreach (var c in Guild.Channels)
                        try
                        {
                            if (!DiscordChannels.ContainsValue(c)) channelsToRemove.Add(c);
                        }
                        catch (NotFoundException exception)
                        {
                            Console.WriteLine(exception.JsonMessage);
                        }

                    foreach (var dm in channelsToRemove) await dm.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            Console.WriteLine("Supr fini");

            await RoleAssignment(msgInv, e, players);

            foreach (var p in PersonnagesList)
            {
                WriteDebug($"Y : {p.Me.Username}");

                DiscordMember usr = GameBuilder.GetMember(Guild, p.Me);

                await DiscordChannels[GameChannel.BotVoice].AddOverwriteAsync(usr, Permissions.None,
                    GameBuilder.CreatePerms(Permissions.AccessChannels, Permissions.UseVoice));
            }


            if (PersonnagesList.Count < 2)
            {
                Victory = Victory.NotPlayable;
                embed = new DiscordEmbedBuilder
                {
                    Title = $"{Texts.NotEnoughPlayer} {PersonnagesList.Count}",
                    Color = Color.InfoColor
                };
                await DiscordChannels[GameChannel.TownText].SendMessageAsync(embed: embed.Build());
            }

            while (Victory == Victory.None && Victory != Victory.NotPlayable) await PlayAsync();
        }

        public async Task RoleAssignment(DiscordMessage msgInv, CommandContext e, List<DiscordMember> players)
        {
            try
            {
                // Création de tous les channels sans Droit
                Game.WriteDebug(Global.currGame);
                var chsPerso = await Global.Games[Global.currGame].Guild
                    .CreateChannelAsync(Global.Games[Global.currGame].Texts.PersoGroup, ChannelType.Category);
                Global.Games[Global.currGame].DiscordChannels.Add(GameChannel.PersoGroup, chsPerso);

                var wolfGrpChannel =
                    await Global.Games[Global.currGame].Guild
                        .CreateChannelAsync(Global.Games[Global.currGame].Texts.WolvesChannel, ChannelType.Category);
                var townGrpChannel = await Global.Games[Global.currGame].Guild
                    .CreateChannelAsync(Global.Games[Global.currGame].Texts.TownChannel, ChannelType.Category);


                var townTChannel =
                    await Guild.CreateChannelAsync(Texts.TownChannel, ChannelType.Text, townGrpChannel);
                var townVChannel =
                    await Guild.CreateChannelAsync(Texts.TownChannel, ChannelType.Voice, townGrpChannel);
                DiscordChannels.Add(GameChannel.TownText, townTChannel);
                DiscordChannels.Add(GameChannel.TownVoice, townVChannel);


                var wolfTChannel =
                    await Guild.CreateChannelAsync(Texts.WolvesChannel, ChannelType.Text, wolfGrpChannel);
                var wolfVChannel =
                    await Guild.CreateChannelAsync(Texts.WolvesChannel, ChannelType.Voice, wolfGrpChannel);
                DiscordChannels.Add(GameChannel.WolfText, wolfTChannel);
                DiscordChannels.Add(GameChannel.WolfVoice, wolfVChannel);

                var graveyardGrpChannel =
                    await Guild.CreateChannelAsync(Texts.GraveyardChannel, ChannelType.Category);
                var graveyardTChannel = await Guild.CreateChannelAsync(Texts.GraveyardChannel,
                    ChannelType.Text, graveyardGrpChannel);
                var graveyardVChannel = await Guild.CreateChannelAsync(Texts.GraveyardChannel,
                    ChannelType.Voice, graveyardGrpChannel);

                await graveyardTChannel.AddOverwriteAsync(Global.Roles[CustomRoles.Spectator], GameBuilder.UsrPerms);

                DiscordChannels.Add(GameChannel.GraveyardText, graveyardTChannel);
                DiscordChannels.Add(GameChannel.GraveyardVoice, graveyardVChannel);

                foreach (var discordMember in Guild.Members)
                    if (discordMember.Roles.Contains(Global.Roles[CustomRoles.Spectator]))
                        await graveyardVChannel.AddOverwriteAsync(discordMember,
                            GameBuilder.CreatePerms(Permissions.UseVoiceDetection, Permissions.UseVoice,
                                Permissions.Speak));

                await GameBuilder.CreatePersonnages(this, players);

                await (await e.Channel.GetMessageAsync(msgInv.Id)).ModifyAsync((await townTChannel.CreateInviteAsync())
                    .ToString());
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task NewGuildMember(GuildMemberAddEventArgs e)
        {
            await e.Member.GrantRoleAsync(Global.Roles[CustomRoles.Spectator]);
        }

        private async Task StartMember(GuildMemberAddEventArgs e)
        {
            var p =
                GameBuilder.CreatePerms(Permissions.AccessChannels, Permissions.UseVoice, Permissions.Speak);
            await DiscordChannels[GameChannel.BotVoice].AddOverwriteAsync(e.Member, p);
            WriteDebug($"D : {e.Member.Username}");
        }



        public void CreateStack()
        {
            Moments = new Stack<Moment>();


            Moments.Push(Moment.Voting);
            Moments.Push(Moment.EndNight); // Tue vraiment les targets 
            if (PersonnagesList.FindAll(p => p.GetType() == typeof(Witch)).Count >= 1)
                Moments.Push(Moment.NightPhase2); // Witch 

            Moments.Push(Moment.NightPhase1); // lg, pf, voyante 


            if (Laps == 1 && PersonnagesList.FindAll(p => p.GetType() == typeof(Cupidon)).Count >= 1)
                Moments.Push(Moment.Cupid);

            Laps++;
        }

        public void CheckVictory()
        {
            // Si il n'y a pas de loup = la ville gagne 
            var nbWolves = PersonnagesList.FindAll(p => p.GetType() == typeof(Wolf) && p.Alive).Count;
            if (nbWolves == 0)
            {
                Victory = Victory.Town;
                DiscordChannels[GameChannel.TownText].SendMessageAsync(Texts.TownVictory);
            }

            // Si il n'y a que des loups = les loups gagne 
            if (nbWolves == PersonnagesList.FindAll(p => p.Alive).Count)
            {
                Victory = Victory.Wolf;
                var embed = new DiscordEmbedBuilder
                {
                    Title = Texts.WolfVictory,
                    Color = Color.WolfColor,
                    ImageUrl = "https://f4.bcbits.com/img/a3037005253_16.jpg"
                };
                DiscordChannels[GameChannel.TownText].SendMessageAsync(embed: embed.Build());
            }

            // On check si les amoureux sont les seuls restant 
            if (PersonnagesList.FindAll(p => p.Effect == Effect.Lover && p.Alive).Count ==
                PersonnagesList.FindAll(p2 => p2.Alive).Count)
            {
                Victory = Victory.Lovers;
                var embed = new DiscordEmbedBuilder
                {
                    Title = Texts.LoverVictory,
                    Color = Color.LoveColor
                };
                DiscordChannels[GameChannel.TownText].SendMessageAsync(embed: embed.Build());
            }

            if (Victory != Victory.None) Moments.Push(Moment.End);
        }


        public static void WriteDebug(object o)
        {
            Console.WriteLine("DEBUG\t" + o);
        }

        public async Task PlayAsync()
        {
            try
            {
                CreateStack();

                WriteDebug($"Laps : {Laps}");

                var done = false;

                while (Moments.Count > 0 && !done)
                {
                    WriteDebug($"Moment Active : {Moments.Peek()}");

                    foreach (var moment in Moments.ToArray()) WriteDebug($"Moment in pile : {moment}");


                    switch (Moments.Pop())
                    {
                        case Moment.Voting:
                            await BotFunctions.DailyVote(this);
                            break;

                        case Moment.HunterDead:
                            await BotFunctions.HunterDeath(this);
                            break;

                        case Moment.EndNight:
                            await BotFunctions.EndNight(this);
                            await BotFunctions.DayAnnoucement(this);
                            break;

                        case Moment.NightPhase1:
                            await BotFunctions.NightAnnoucement(this);
                            await BotFunctions.WolfVote(this);
                            await BotFunctions.SeerAction(this);
                            await BotFunctions.LittleGirlAction(this);
                            break;

                        case Moment.NightPhase2:
                            await BotFunctions.WitchMoment(this);
                            break;

                        case Moment.Election:
                            await BotFunctions.Elections(this);
                            break;

                        case Moment.Cupid:
                            await BotFunctions.CupidonChoice(this);
                            break;

                        case Moment.End:
                            done = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Ending()
        {
        }


        public async Task Kill(Personnage p)
        {
            try
            {
                p.Alive = false;
                await p.Me.PlaceInAsync(DiscordChannels[GameChannel.GraveyardVoice]);
                var embed = new DiscordEmbedBuilder
                {
                    Color = Color.DeadColor,
                    Title = $"{p.Me.Username} {Texts.DeadMessagePublic} {p.GetClassName()}"
                };

                await DiscordChannels[GameChannel.TownText].SendMessageAsync(embed: embed.Build());


                foreach (var discordChannel in DiscordChannels.Values)
                    await discordChannel.AddOverwriteAsync(p.Me, Permissions.AccessChannels);

                await p.Me.RevokeRoleAsync(Global.Roles[CustomRoles.Player]);
                await p.Me.GrantRoleAsync(Global.Roles[CustomRoles.Spectator]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}