using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct ActionUD
{
	public UnityEvent ActionUp;
	public UnityEvent ActionDown;
}

//a serializable class to store the input , actions and a index to the action to execute like a state
[Serializable]
public struct InputAction
{
	public string name;
	public List<ActionUD> Actions;
	public int ActionIndex;
}


public class PlayerInputManager : MonoBehaviour
{
	//hash map
	private Dictionary<string, InputAction> inputMap = new Dictionary<string, InputAction>();

	public List<InputAction> InputActions;
	
	//caches
	string _InputString = "";
	InputAction _ActionBuffer;


	
	void Awake()
	{
		for (int i = 0; i < InputActions.Count; i++)
		{
			_ActionBuffer = InputActions[i];		
			AddInput(
				_ActionBuffer.name,
				_ActionBuffer
				);
		}

		#if UNITY_EDITOR
		//Debugging
		//log the ammount of input actions assigned
		Debug.Log($"Input Actions: {InputActions.Count} set");
		#endif
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
					HandleActions(_ActionBuffer.Actions,InputType.Down);
				}
				else if (Input.GetButtonUp(_InputString))
				{
					//execute actions given 
					HandleActions(_ActionBuffer.Actions,InputType.Up);
				}
			}	
    	}

	    public void SetInputIndex(string inputName, int index)
	    {
		    // Check if the input action exists in the dictionary
		    if (inputMap.ContainsKey(inputName))
		    {
			    // Get the input action
			    InputAction inputAction = inputMap[inputName];

			    // Update the action index
			    inputAction.ActionIndex = index;

			    // Update the input action in the dictionary
			    inputMap[inputName] = inputAction;
		    }
		    else
		    {
			    Debug.LogWarning($"Input action '{inputName}' not found in the input map.");
		    }
	    }
	void HandleActions(List<ActionUD> actionBuffer,InputType type)
	{
		for(int i = 0; i < actionBuffer.Count; i++)
		{
			//if the action index is null dont execute

			switch (type)
			{
				case InputType.Up:
					if(actionBuffer[i].ActionUp == null) continue;
					actionBuffer[i].ActionUp?.Invoke();

					break;
				case InputType.Down: 
					if(actionBuffer[i].ActionDown == null) continue;
					//invoke
					actionBuffer[i].ActionDown?.Invoke();
					
					break;
			}
		}
		
		

	}
	public enum InputType
	{
		Up,
		Down
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
