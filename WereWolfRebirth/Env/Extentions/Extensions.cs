using System.Text.RegularExpressions;
using DSharpPlus.Entities;

namespace WereWolfRebirth.Env.Extentions
{
    public static class StringExtention
    {
        public static string RemoveSpecialChars(this string str) => Regex.Replace(str, "[^a-zA-Z0-9]", "");
    }

    public static class DiscordUserExtention
    {
        public static DiscordMember GetMember(this DiscordUser usr) => Game.Guild.GetMemberAsync(usr.Id).GetAwaiter().GetResult();
    }
}