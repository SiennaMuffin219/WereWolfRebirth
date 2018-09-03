using DSharpPlus.Entities;
using WereWolfRebirth.Env;

namespace WereWolfRebirth.Locale
{
    public class Language
    {
        public string Lang { get; set; }

        public string BotName { get; set; }

        public string Player { get; set; }
        public string Spectator { get; set; }
        public string Admin { get; set; }

        public string GraveyardChannel { get; set; }
        public string TownChannel { get; set; }
        public string WolvesChannel { get; set; }
        public string PersoGroup { get; set; }

        public string CitizenToString { get; set; }
        public string WolfToString { get; set; }
        public string HunterToString { get; set; }
        public string SaviorToString { get; set; }
        public string WitchToString { get; set; }
        public string SeerToString { get; set; }
        public string TalkativeSeerToString { get; set; }
        public string LittleGirlToString { get; set; }
        public string CupidToString { get; set; }
        public string TownFriendly { get; set; }
        public string CharmedMessage { get; set; }
        public string MayorMessage { get; set; }

        public string DailyVoteMessage { get; set; }
        public string NightlyWolfMessage { get; set; }
        public string CitizenName { get; set; }
        public string WolfName { get; set; }
        public string HunterName { get; set; }
        public string LittleGirlName { get; set; }
        public string CupidName { get; set; }
        public string SeerName { get; set; }
        public string TalkativeSeerName { get; set; }
        public string WitchName { get; set; }
        public string SaviorName { get; set; }
        public string DeadMessagePrivate { get; set; }
        public string DeadMessagePublic { get; set; }
        public string HunterDeathQuestion { get; set; }
        public string PublicHunterMessage { get; set; }
        public string LoveSuicide { get; set; }
        public string SeerRecMsg { get;  set; }
        public string SeerAskMsg { get;  set; }
        public string CupidMessage { get; set; }
        public string WitchSaveMsg { get; set; }
        public string WitchKillMsg { get; set; }
        public string TownVictory { get; set; }
        public string LoverVictory { get; set; }
        public string WolfVictory { get; set; }
        public string NightAnnoucement { get; set; }
        public string DayAnnoucement { get; set; }
        public string NotEnoughPlayer { get; set; }
        public string BotWantPlay { get; set; }

        public static string FirstDieMessages(DiscordMember dm)
        {
            var str = "";

            switch (Game.Texts.Lang)
            {
                case "fr":
                    str = $"{dm.DisplayName} est mort, il était ";
                    break;

                case "en":
                    str = $"{dm.DisplayName} is dead, he was";
                    break;
            }

            return str;
        }

    }

}