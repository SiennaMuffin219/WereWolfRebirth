using System;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.Entities;

public class Personnage
{

    public DiscordUser Me { get; private set; }
    public Role PlayerRole { get; private set; }
    public bool Alive {get; set;}
    public Bonus bonus = Bonus.None;

    
    public Personnage(DiscordUser me, Role role)
    {
        Me = me;
        PlayerRole = role;
														
	}

	public static List<Personnage> CreatePersonnages(List<DiscordUser> players)
	{
        List<Personnage> personnages = new List<Personnage>();

		List<Role> roles = CreateRoles(players.Count);


		foreach (DiscordUser user in players)
		{
            Random rand = new Random();
			int NbRand = rand.Next(roles.Count);
            personnages.Add(new Personnage(user, roles[NbRand]));
            roles.RemoveAt(NbRand);
		}

        return personnages;
	}



    static public List<Role> CreateRoles(int NbPlayer)
    {
        List<Role> RoleList = new List<Role>();

        for (int i = 0; i < NbPlayer; i++)
        {
            switch (i)
            {
                case 1:
                    RoleList.Add(Role.Villageois); break;
                case 2:
                    RoleList.Add(Role.Loup); break;
                case 3:
                    RoleList.Add(Role.Voyante); break;
                case 4:
                    RoleList.Add(Role.Loup); break;
                case 5:
                    RoleList.Add(Role.Salvateur);
                    RoleList.Add(Role.Villageois);
                    RoleList.Add(Role.Loup); break;
                case 6:
                    RoleList.Add(Role.PetiteFille); break;
                case 7:
                    RoleList.Add(Role.Sorciere); break;
                case 8:
                    RoleList.Add(Role.Chasseur); break;
                case 9:
                    RoleList.Add(Role.Loup); break;
                case 10:
                    RoleList.Add(Role.Cupidon); break;
                default:
                    RoleList.Add(i % 2 == 0 ? Role.Loup : Role.Villageois); break;
            }
        }
        return RoleList;
    }
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
public enum Bonus
{
    None = 0,
    Amoureux = 1, 
    Capitaine = 2,
    Charmed = 4,
    
}

public enum Role
{
	Villageois,
	Sorciere,
	Voyante,
	Cupidon, 
	Salvateur,
	PetiteFille,
	Chasseur,
	
	Loup
}

//"Vous êtes " + (Role == RolesNames.Villageois || Role == RolesNames.LoupGarou ? "un " : "le/la ") + RoleToString() + ".\nConservez ce message préciseusement et ne révélez son contenu à personne !\nPour plus d'informations sur votre personnage, envoyez *!info* dans ce salon privé.\nBonne chance et bonne partie."
// Message = "... ben... d'être un villageois... Et le rester le plus longtemps possible.\nPour cela vous devez éliminer les loups garous qui pourraient décider de **vous** éliminer et le seul moyen de le faire est de voter à la fin de la journée contre celui qui vous semble le plus suspect.\nEt aussi de réussir à ce que les autres villageois ne votent pas votre mort.\nAvouez que ça serait dommage."
// Message = 
// return (Role == RolesNames.LoupGarou ? "Loup Garou" : Role == RolesNames.PetiteFille ? "Petite Fille" : Role.ToString());
