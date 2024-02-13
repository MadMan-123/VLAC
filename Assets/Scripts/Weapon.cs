using System;
using UnityEngine;

public class Weapon : Item
{
	public int Damage;
	public float Range;
	public float AttackDelay;
	public float Force;
	protected bool CanAttack = true;
	

	

}



public enum WeaponType
{
	MELEE = 0,
	FIREARM = 1
}
