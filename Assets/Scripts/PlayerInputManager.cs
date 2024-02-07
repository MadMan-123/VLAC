using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputManager : MonoBehaviour
{
	//hash map
	Dictionary<string, List<UnityEvent>> InputMap = new Dictionary<string, List<UnityEvent>>();

	//Input Paramaters
	public List<UnityEvent> FireActions;

	//caches
	string _InputString = "";
	List<UnityEvent> _ActionBuffer;	
    	
	
		void Start()
		{
			AddInput("Fire1",FireActions); 
		}

    	void Update()
    	{
		//get the Key Value Paring from Input Map
		foreach(var kvp in InputMap)
		{
			//set the caches
			_InputString = kvp.Key;
			_ActionBuffer = kvp.Value;
			
			//ask if the input is being pressed
			if(Input.GetButtonDown(_InputString))
			{
				//execute actions given 
				HandleActions(_ActionBuffer);
			}
		}	
    	}

	void HandleActions(List<UnityEvent> ActionBuffer)
	{
		//for each action
		foreach(var action in ActionBuffer)
		{
			//invoke
			action?.Invoke();
		}

	}

	void AddInput(string InputName, List<UnityEvent> ActionBuffer)
	{
		
		if(InputMap.ContainsKey(InputName))
		{
			//input mapping exists
			return;
		}

		//input dosent exist

		InputMap.Add(InputName, ActionBuffer);
	
	}




}
