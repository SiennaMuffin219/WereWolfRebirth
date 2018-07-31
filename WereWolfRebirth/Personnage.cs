using System;

public class Personnage
{

	public string Name {get; private set;}
	public Role Role {get; private set;}
	public Personnage(string name, Role role)
	{
		this.Name = name;
		this.Role = role;
	}



}


public enum Role
{
	Villagois,
	Loup
}