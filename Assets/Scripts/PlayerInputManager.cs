using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//a serializable class to store the input , actions and a index to the action to execute like a state
[Serializable]
public struct InputAction
{
	public string name;
	public List<UnityEvent> ActionsDown;
	public int ActionIndex;
}


public class PlayerInputManager : MonoBehaviour
{
	//hash map
	private readonly Dictionary<string, InputAction> inputMap = new Dictionary<string, InputAction>();

	public List<InputAction> InputActions;

	//caches
	string _InputString = "";
	InputAction _ActionBuffer;


	
	void Start()
	{
		for (int i = 0; i < InputActions.Count; i++)
		{
			_ActionBuffer = InputActions[i];		
			AddInput(
				_ActionBuffer.name,
				_ActionBuffer
				);
		}
	}

    	void Update()
    	{
			//get the Key Value Paring from Input Map
			foreach(var kvp in inputMap)
			{
				//set the caches
				_InputString = kvp.Key;
				_ActionBuffer = kvp.Value;
				
				//ask if the input is being pressed
				if(Input.GetButtonDown(_InputString))
				{
					//execute actions given 
					HandleActions(_ActionBuffer.Actions);
				}
			}	
    	}

	void HandleActions(List<UnityEvent> actionBuffer)
	{
		//for each action
		foreach(var action in actionBuffer)
		{
			//invoke
			action?.Invoke();
		}

	}

	void AddInput(string inputName, InputAction action)
	{
		
		if(inputMap.ContainsKey(inputName))
		{
			//input mapping exists
			return;
		}

		//input dosent exist

		inputMap.Add(inputName, action);
	
	}




}
