using System.Security.Cryptography;

namespace WereWolfRebirth.Env
{
    public class Vote
    {
        public string Owner { get; set; }
        public string To { get; set; }
        public Vote(string owner, string to)
        {
            Owner = owner;
            To = to;
        }

       
    }
}