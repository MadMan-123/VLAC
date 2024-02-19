using System;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{	
	//current health
	[SerializeField] private int CurrentHealth = 0;
	 
	//max health
	public int MaxHealth {private set; get;} = 100;

	
	public Action OnDeath;
	

	//Heal
	public void Heal(int HealBy)
	{
		//work out how much has healed
		int cache = MaxHealth - (CurrentHealth + HealBy);
		//set the health 
		SetCurrentHealth(cache);
	}

	//Damage
	public void Damage(int DamageAmmount)
	{
		//take the damage 
		CurrentHealth -= DamageAmmount;
		
		//call the on death action if needed
		if(CurrentHealth <= 0)
		{
			//call on death		
			OnDeath?.Invoke();
		}


	}
	
	public void Damage(int DamageAmmount, Transform Source)
	{
		//take the damage 
		CurrentHealth -= DamageAmmount;
		//call on damage
		
		//call the on death action if needed
		if(CurrentHealth <= 0)
		{
			//call on death		
			OnDeath?.Invoke();
		}

	}
	
	//SetMax
	public void SetMaxHealth(int NewMaxHealth)
	{
		MaxHealth = NewMaxHealth;
		//if in dev mode log the new max health
		#if UNITY_EDITOR
			Debug.Log($"{gameObject.name} Max health has been set to: {MaxHealth}");
		#endif
	}
	//SetCurrent
	public void SetCurrentHealth(int NewCurrentHealth)
	{
		CurrentHealth = NewCurrentHealth;
		#if UNITY_EDITOR
			Debug.Log($"{gameObject.name} health has been set to: {CurrentHealth}");

		#endif
	}

}
