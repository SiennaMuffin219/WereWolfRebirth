using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Locale;
using WereWolfRebirth.Roles;

namespace WereWolfRebirth.Env 
{
    class Game
    {
        public static Dictionary<string, DiscordChannel> DiscordChannels;
        public static Language TextJson = JsonConvert.DeserializeObject<Language>(File.ReadAllText($@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}/Locale/Fr/lang.json", System.Text.Encoding.UTF8));
        public static bool wait = true;
        public static List<Personnage> PersonnagesList;
        public static Victory Victory = Victory.None;
        public static DiscordClient Client = null;
        public static List<Vote> VoteList;



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

        public static DiscordGuild guild = null;

       


        public static Permissions CreatePerms(params Permissions[] perms)
        {
            return GrantPerm(Permissions.None, perms);
        }


        public static Permissions GrantPerm(Permissions p, params Permissions[] grant)
        {
            foreach (Permissions pg in grant)
            {
                p |= pg;
            }

            return p;
        }

        public static Permissions RevokePerm(Permissions p, params Permissions[] grant)
        {
            foreach (Permissions pg in grant)
            {
                p &= ~pg;
            }
            return p;
        }

        public static void Play(Queue<Enum.Moments> moments)
        {
            while (moments.Count > 0)
            {
                
            } 
        }

    }
}