using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour
{
    [SerializeField] private AmmoType type;
    [SerializeField] private int amount;
    public void AddAmmunition(GameObject Source)
    {
        //get the inventory of the source
        if(!Source.TryGetComponent(out Inventory inventory))
        {
            Debug.LogWarning("No Inventory Found on Source");
            return;
        }
        
        //add the ammunition to the player's inventory
        inventory.AddAmmo(type,amount);
        
        Debug.Log("Added " + amount + " " + type + " to " + Source.name);
        //disable the ammunition
        gameObject.SetActive(false);
    }
    
}

public enum AmmoType
{
    BuckAndBall,
    GrapeShot,
    Explosive,
    ArtilleryShell
}
