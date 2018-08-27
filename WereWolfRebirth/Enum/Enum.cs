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
        Mayor = 2
        // Charmed = 4
        
    }

    public enum Victory
    {
        None,   // Pas encore terminé
        Wolf,   // Les loups
        Town,   // Le Village
        Lovers // Les Amoureux

        //  PiedPiper, // Joueur de flute

    }

    public enum Moments
    {
        Voting,         // Vote du jour 
        HunterDead,     // Vengeance du Chasseur
        Seer,           // Action de la voyante
        Witch,          // Action de la Witch
        Wolfs,          // Wolfs  & Petite fille 
        End,            // Fin de la partie
        Election,       // Election du maire 
        Cupid           // Cupidon tour 1

    }


}