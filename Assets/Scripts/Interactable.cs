using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Interactable : MonoBehaviour
{
    public UnityEvent<GameObject> OnInteract;
    public Item ConnectedItem;
    public bool HasItem = false;
    public GameObject InteractSource;

    private void Start()
    {
        HasItem = TryGetComponent(out ConnectedItem);
    }

    public void Interact(GameObject Source)
    {
        InteractSource = Source;
        if (OnInteract != null)
        {
            OnInteract.Invoke(Source);
            #if UNITY_EDITOR
            Debug.Log("Interacted with " + OnInteract.GetPersistentMethodName(0) + " on " + gameObject.name + " from " + Source.name);
            #endif            
        }
        else
            Debug.LogWarning("No Interact Event Set for interaction  ");
    }
}

