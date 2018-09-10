using System.Text.RegularExpressions;

namespace BotDiscord.Env.Extentions
{
    public static class StringExtention
    {
        public static string RemoveSpecialChars(this string str) => Regex.Replace(str, "[^a-zA-Z0-9]", "");
    }

 
}