using System;

namespace WereWolfRebirth.Enum 
{
    public enum Role 
    {
        Citizien,       // Villageois
        Seer,           // Voyante
        TalkativeSeer,  // Voyante bavarde
        LittleGirl,     // Petite Fille
        Hunter,         // Chasseur
        Witch,          // Sorcière
        Cupid,          // Cupidon
        Savior,         // Salvateur
        
        Wolf,           // Loup

        None            // None

    }

    
    public enum Effect
    {
        None = 0,
        Lover = 1, 
        Mayor = 2,
        // Charmed = 4
        
    }

    public enum Victory
    {
        None,   // Pas encore terminé
        Wolf,   // Les loups
        Town,   // Le Village
        Lovers, // Les Amoureux

        //  PiedPiper, // Joueur de flute

    }

    public enum Moments
    {
        Voting,
        
    }


}