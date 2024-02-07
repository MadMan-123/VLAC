using System;
using UnityEngine;

public class Weapon : Item
{
	protected WeaponType type;
	protected int Damage;
	protected float Range;
	protected float AttackDelay;
	protected float Force;
	protected bool CanAttack = true;

	public override void Use()
	{
		OnAttack();
	}

	protected virtual void OnAttack()
	{
		
	}
}

public enum WeaponType
{
	MELEE = 0,
	FIREARM = 1
}
