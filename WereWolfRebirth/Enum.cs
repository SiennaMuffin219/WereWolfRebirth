using System;

namespace WereWolfRebirth.Enum 
{
    public enum Role 
    {
        Citizien,
        Seer,
        TalkativeSeer,
        LittleGirl,
        Hunter,
        Witch,
        Cupid,
        Savior,
        
        Wolf,

        None

    }

    
    public enum Bonus
    {
        None = 0,
        Lover = 1, 
        Mayor = 2,
        Charmed = 4
        
    }

        public enum Victory
    {
        None, // Pas encore terminé
        Wolf, // Les loups
        Town, // Le Village
        Lovers, // Les Amoureux

        PiedPiper, // Joueur de flute

    }

    public enum Moments
    {
        Voting,
        
    }


}