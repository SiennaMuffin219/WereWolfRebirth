using System;
using System.Collections.Generic;
using System.IO;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using WereWolfRebirth.Enum;
using WereWolfRebirth.Roles;
using WereWolfRebirth;


namespace WereWolfRebirth.Environment 
{
    static class Game
    {
        public static Dictionary<string, DiscordChannel> channels;
        public static Language langJson  = JsonConvert.DeserializeObject<Language>(File.ReadAllText(@"./Locale/Fr/lang.json", System.Text.Encoding.UTF8), new JsonSerializerSettings() { Culture = System.Globalization.CultureInfo.GetCultureInfo("fr-FR") });
        public static bool wait = true;
        public static List<Personnage> personnages;
        public static Victory victory = Victory.None;
        public static void CheckVictory() 
        {
            if (personnages.Exists(x => 
                    x.GetType() == Type.GetType("WereWolfRebirth.Roles.Citizien") || 
                    x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Citizien"))))
            {
                Citizien.HasWon();
            }

            if (personnages.Exists(x =>  
                    x.GetType() == Type.GetType("WereWolfRebirth.Roles.Wolf") || 
                    x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Wolf"))))           
            {
                Wolf.HasWon();
            }

            #region Not Implemented Yet        
            // if(personnages.Exists(x => x.bonus == Bonus.Amoureux)       
            // {
            //     Lovers.HasWon();
            // }

        
            // if(personnages.Exists(x => 
            //         x.GetType() == Type.GetType("WereWolfRebirth.Roles.PiedPiper") || 
            //         x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.PiedPiper"))))            {
            //     PiedPiper.HasWon();
            // }
            #endregion
        


        }
    }

}