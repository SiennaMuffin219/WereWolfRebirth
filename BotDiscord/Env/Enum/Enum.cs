namespace BotDiscord.Env.Enum
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
        Wolf // None

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
        Lovers,	// Les Amoureux

        NotPlayable
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
        NightPhase1, // Wolfs & Petite fille & Voyantes  

        End
    }
    public enum GameChannel
    {
		PersoGroup,
		TownText,
		TownVoice,
		WolfText,
		WolfVoice,
		GraveyardText,
		BotText,
        GraveyardVoice,
        BotVoice
    }
    public enum CustomRoles
    {
        Spectator,
        Player,
        Admin
    }
}




















