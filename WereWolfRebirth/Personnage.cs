using System;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.Entities;
using WereWolfRebirth.Roles;

public abstract class Personnage
{

    public DiscordUser Me { get; private set; }
    public bool Alive {get; set;}
    public Bonus bonus = Bonus.None;    
    public Personnage(DiscordUser me) => Me = me;
  



    



    /*

    
    public override string ToString()
    {
        string str = $"Bonjour {Me.Username}. \nVous êtes {(PlayerRole == Role.Villageois || PlayerRole == Role.Loup ? "un " : "le / la ")}. \nVotre but est de";

        switch (PlayerRole)
        {
            case Role.Villageois:
                str += "... ben... d'être un villageois... Et de le rester le plus longtemps possible.\nPour cela vous devez éliminer les loups garous qui pourraient décider de **vous** éliminer et le seul moyen de le faire est de voter à la fin de la journée contre celui qui vous semble le plus suspect.\nEt aussi de réussir à ce que les autres villageois ne votent pas votre mort.\nAvouez que ça serait dommage.";
                break;

            case Role.Sorciere:
                str += "Vous faites partie du village ";
                break;

            case Role.Voyante:
                break;

            case Role.Cupidon:
                break;
            
            case Role.PetiteFille:
                str += "sale pute tuez vous vouala c:";
                 break;
            case Role.Chasseur:
                break;
            case Role.Loup:
                str += " TUER TOUT LE MONDE !! Enfin, tout le monde sauf vous et les autres loups !\nDonc en gros vous devez tuer les villageois.\n*\"Mais comment je peux faire ça ?\"* allez-vous me demander, hé bien c'est très simple : la nuit, quand le village est endormi, allez rejoindre vos amis loups-garous (si vous avez des amis) et choisissez une victime.\nMais faites attention, la petite fille (qui n'arrive pas à dormir) est peut-être en train de vous espionner !";
                break;
            default:
                break;
        }

        return str;
    }    
}
 */


}

//"Vous êtes " + (Role == RolesNames.Villageois || Role == RolesNames.LoupGarou ? "un " : "le/la ") + RoleToString() + ".\nConservez ce message préciseusement et ne révélez son contenu à personne !\nPour plus d'informations sur votre personnage, envoyez *!info* dans ce salon privé.\nBonne chance et bonne partie."
// Message = "... ben... d'être un villageois... Et le rester le plus longtemps possible.\nPour cela vous devez éliminer les loups garous qui pourraient décider de **vous** éliminer et le seul moyen de le faire est de voter à la fin de la journée contre celui qui vous semble le plus suspect.\nEt aussi de réussir à ce que les autres villageois ne votent pas votre mort.\nAvouez que ça serait dommage."
// Message = 
// return (Role == RolesNames.LoupGarou ? "Loup Garou" : Role == RolesNames.PetiteFille ? "Petite Fille" : Role.ToString());
