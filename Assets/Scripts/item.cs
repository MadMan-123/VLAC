using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	public Inventory CurrentInventory;
	private void Start()
	{
		TryGetComponent(out rb);
	}

	public virtual void Use()	{	}
	
	public void PickUp(GameObject Source)
	{
		if(!Source.TryGetComponent(out Inventory inventory))
		{
			#if UNITY_EDITOR
			Debug.Log("No Inventory Found on Source");
			#endif
		}
		if (inventory == null)
		{
			#if UNITY_EDITOR
				Debug.LogWarning("No Inventory Found");
			#endif
				return;
		}
		CurrentInventory = inventory;
		//add the item to the inventory
		inventory.AddItem(this);
		//disable the item
		gameObject.SetActive(false);
		//stop the item from being affected by physics
		rb.isKinematic = true;
	}
	

	public void Drop()
	{
		gameObject.SetActive(true);
		rb.isKinematic = false;
	}
}

