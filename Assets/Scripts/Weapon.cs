using System;
using UnityEngine;

public class Weapon : Item
{
	WeaponType type;
	int Damage;
	float Range;
	float AttackDelay;
	float Force;
		

	public override void Use()
	{
		OnAttack();
	}

	public virtual void OnAttack()
	{
	}
}

public enum WeaponType
{
	MELEE = 0,
	FIREARM = 1
}
