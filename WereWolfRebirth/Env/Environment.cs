using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Roles;
using WereWolfRebirth.Locale;

namespace WereWolfRebirth.Env
{
    internal static class Game
    {
        public static Dictionary<GameChannel, DiscordChannel> DiscordChannels;
        public static Language TextJson = null;
        public static bool wait = true;
        public static List<Personnage> PersonnagesList;
        public static Victory Victory = Victory.None;
        public static DiscordClient Client = null;
        public static List<Vote> VoteList;
        public static ulong GuildId;
        public static DiscordGuild guild = null;


        public static void SetLanguage(string lang)
        {
            TextJson = JsonConvert.DeserializeObject<Language>(File.ReadAllText($@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}/Locale/{lang}/lang.json", Encoding.UTF8));

        }


        public static void CheckVictory()
        {
            if (PersonnagesList.Exists(x =>
                    x.GetType() == Type.GetType("WereWolfRebirth.Roles.Citizien") ||
                    x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Citizien"))))
            {
                Citizien.HasWon();
            }

            if (PersonnagesList.Exists(x =>
                    x.GetType() == Type.GetType("WereWolfRebirth.Roles.Wolf") ||
                    x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Wolf"))))
            {
                Wolf.HasWon();
            }

            #region Not Implemented Yet        
            // if(PersonnagesList.Exists(x => x.bonus == Bonus.Amoureux)       
            // {
            //     Lovers.HasWon();
            // }


            // if(PersonnagesList.Exists(x => 
            //         x.GetType() == Type.GetType("WereWolfRebirth.Roles.PiedPiper") || 
            //         x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.PiedPiper"))))            {
            //     PiedPiper.HasWon();
            // }
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

        public static void Play(Queue<Moments> moments)
        {
            while (moments.Count > 0)
            {
                
            } 
        }

        public static async Task SetSpectatorAsync(Personnage p)
        {
            p.Alive = false;


            foreach (var discordChannel in DiscordChannels.Values)
            {
                await discordChannel.AddOverwriteAsync(p.Me as DiscordMember, Permissions.AccessChannels);

            }

            await DiscordChannels[GameChannel.GraveyardVoice].PlaceMemberAsync(p.Me as DiscordMember);

            Permissions spectPermissions = CreatePerms(Permissions.AccessChannels, Permissions.UseVoice,
                Permissions.AddReactions, Permissions.ReadMessageHistory, Permissions.CreateInstantInvite,
                Permissions.Speak, Permissions.SendMessages, Permissions.SendTtsMessages, Permissions.UseVoiceDetection,
                Permissions.AttachFiles);

            await DiscordChannels[GameChannel.GraveyardText].AddOverwriteAsync(p.Me as DiscordMember, spectPermissions);
            await DiscordChannels[GameChannel.GraveyardVoice].AddOverwriteAsync(p.Me as DiscordMember, spectPermissions);


        }

    }
}