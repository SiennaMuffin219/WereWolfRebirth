namespace WereWolfRebirth.Enum
{
    public enum GameRole 
    {
        Citizen,       // Villageois
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
        Lovers	// Les Amoureux

        //  PiedPiper, // Joueur de flute

    }

    public enum Moment
    {
        Voting,         // Vote du jour 
        HunterDead,     // Vengeance du Chasseur
   
        Election,       // Election du maire 
        Cupid,           // Cupidon tour 1

        EndNight,

        NightPhase2, // Action de la Witch
        NightPhase1 // Wolfs & Petite fille & Voyantes  
    }

    public enum GameChannel
    {
		PersoGroup,
		SharedGroup,
		TownText,
		TownVoice,
		WolfText,
		WolfVoice,
		GraveyardText,
		BotText
    }

    public enum CustomRoles
    {
        Spectator,
        Player,
        Admin
    }
}