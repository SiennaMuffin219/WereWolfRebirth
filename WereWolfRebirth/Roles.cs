using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace WereWolfRebirth.Roles
{
    static class GameBuilder
    {
        public static void CreatePersonnages(List<DiscordUser> players)
        {
            try
            {
                
                
                List<Role> roles = CreateRoles(players.Count);
                Random rand = new Random(DateTime.Now.Millisecond);
                int userId = 0;    


                Game.personnages = new List<Personnage>();
            
                while(players.Count != 0)
                {
                    int nbRand =  rand.Next(roles.Count);
                    
                    
                    
                    switch(roles[nbRand])
                    {
                    case Role.Villageois:
                        Game.personnages.Add(new Citizien(players[userId]));
                    break;
                    case Role.Chasseur:
                        Game.personnages.Add(new Hunter(players[userId]));
                    break;
                    case Role.Cupidon:
                        Game.personnages.Add(new Cupidon(players[userId]));
                    break;
                    case Role.Sorciere:
                        Game.personnages.Add(new Witch(players[userId]));
                    break;
                    case Role.Salvateur:
                        Game.personnages.Add(new Salvator(players[userId]));
                    break;
                    case Role.Voyante:
                        Game.personnages.Add(new Seer(players[userId]));
                    break;                  
                    case Role.VoyanteBavarde:
                        Game.personnages.Add(new TalkativeSeer(players[userId]));
                    break;
                    case Role.PetiteFille:
                        Game.personnages.Add(new LittleGirl(players[userId]));
                    break;

                    case Role.Loup:
                        Game.personnages.Add(new Wolf(players[userId]));
                    break;

                    }
                    roles.RemoveAt(nbRand);
                    players.RemoveAt(userId);

                }
            }
            catch (System.Exception ex1)
            {
                
                System.Console.WriteLine(ex1);
            }


        
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
                        RoleList.Add(i % 3 == 0 ? Role.Loup : Role.Villageois); break;
                }
            }
            return RoleList;
        }

        static public void Debug()
        {
            if(Game.personnages is null)
            {
                Console.WriteLine("Il n'y a aucun personnage joueur dans le jeu");
            }
            else
            {

                int i = 0;
                foreach(Personnage p in Game.personnages)
                {
                    Console.WriteLine(i + " : " + p.ToString());
                    i++;
                }

            }
        }

    }


    #region Role's Classes


    class Wolf : Personnage
    {
        public Wolf(DiscordUser me) : base(me)
        {}

        public void doRole()
        {
            throw new NotImplementedException();
        }

        
        public static bool HasWon() 
        {
            return !Game.personnages.Exists(x =>
                x.GetType() == Type.GetType("WereWolfRebirth.Roles.Citizien") ||
                x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Citizien")));        
        
        }
    }
    
    
    class Citizien :  Personnage
    {


        public Citizien(DiscordUser me) : base(me)
        {}

        public void doRole()
        {
            
        }

        public static  bool HasWon() 
        {
            return !Game.personnages.Exists(x =>
                x.GetType() == Type.GetType("WereWolfRebirth.Roles.Wolf") ||
                x.GetType().IsSubclassOf(Type.GetType("WereWolfRebirth.Roles.Wolf")));        
        }

   
    }


    
    class Salvator : Citizien
    {
        public Salvator(DiscordUser me) : base(me)
        {
        }
    
        public new void doRole()
        {
            throw new NotImplementedException();
        }

        public static new bool HasWon() => Citizien.HasWon();

    }


    class Witch : Citizien
    {
        public Witch(DiscordUser me) : base(me)
        {}
        public static new bool HasWon() => Citizien.HasWon();
    }

    class LittleGirl : Citizien
    {
        public LittleGirl(DiscordUser me) : base(me)
        {}
        public static new bool HasWon() => Citizien.HasWon();
    }

    class Hunter : Citizien
    {
        public Hunter(DiscordUser me) : base(me)
        {}
        public static new bool HasWon() => Citizien.HasWon();
    }

    class Cupidon : Citizien
    {
        public Cupidon(DiscordUser me) : base(me)
        {}
        public static new bool HasWon() => Citizien.HasWon();
    }

    class Seer : Citizien
    {
        public Seer(DiscordUser me) : base(me)
        {}
        public static new bool HasWon() => Citizien.HasWon();
    }

    class TalkativeSeer : Seer
    {
        public TalkativeSeer(DiscordUser me) : base(me)
        {}
        public static new  bool HasWon() => Seer.HasWon();
    }

    #endregion

    

    public enum Role 
    {
        Villageois,
        Voyante,
        VoyanteBavarde,
        PetiteFille,
        Chasseur,
        Sorciere,
        Cupidon,
        Salvateur,
        
        Loup,

        None

    }

    
    public enum Bonus
    {
        None = 0,
        Amoureux = 1, 
        Capitaine = 2,
        Charmed = 4
        
    }

}