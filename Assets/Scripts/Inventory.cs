using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //3 item slots
    public Item[] items = new Item[maxItem];
    [SerializeField] private int itemIndex = 0;
    [SerializeField] private CinemachineVirtualCamera cameraCache;
    public Transform aimPosition; // Position to aim at
    public Transform hipShotPosition; // Position for hip shot
    public float duration = 0.5f; // Speed of lerping to aim position


    private static readonly int maxItem = 3; 
    private int buckAndBall = 0;
    private int grapeShot = 0;
    private int artilleryShell = 0;
    private int explosive = 0;

    private void Start()
    {
        for(int i = 0; i < maxItem; i++)
        {
            if(i == itemIndex ) 
                items[i].gameObject.SetActive(true);
        }
    }

    public void SwitchItem()
    {
        if(AimCoroutine != null)
            StopCoroutine(AimCoroutine);
        
        items[itemIndex].gameObject.SetActive(false);

        if (itemIndex == maxItem - 1)
        {
            itemIndex = 0;
        }
        else
        {
            //set on the next item
            itemIndex++;
        }
        
        

        items[itemIndex].gameObject.SetActive(true);

        
    }

    public void UseCurrentItem()
    {
        
        items[itemIndex].Use();
    }
    
    public void AddItem(Item item)
    {
        if (itemIndex >= maxItem || items[itemIndex] != null) return;
        items[itemIndex] = item;
        itemIndex++;
    }

    public void TryAim(bool aim)
    {
        if(AimCoroutine != null)
            StopCoroutine(AimCoroutine);
        AimCoroutine = StartCoroutine(TryAimCurrentItem(aim));
    }
    
    Coroutine AimCoroutine;
    private IEnumerator TryAimCurrentItem(bool aim)
    {
        //if aim is true then Aim() else HipShot()
        
        
        
        if (items[itemIndex].GetType() == typeof(Firearm))
        {
            Firearm firearm = ((Firearm)items[itemIndex]);
            if (aim)
            {
                Aim(firearm.aimFOV);
            }
            else
            {
                HipShot(firearm.hipShotFOV);
            }
        }
        
        yield break;
    }
    
    private Coroutine currentLerpCoroutine;

    // Method to aim the firearm
    public void Aim(float aimFOV)
    {
        if(currentLerpCoroutine != null)
            StopCoroutine(currentLerpCoroutine);
        //set fov
        LerpFOV(aimFOV);
        //stop hip shot 
        currentLerpCoroutine = StartCoroutine(LerpToPosition(items[itemIndex].transform, aimPosition, duration));
        

    }

    // Method for hip shot
    public void HipShot(float hipShotFOV)
    {
        // Stop any ongoing aim animation
        if (currentLerpCoroutine != null)
        {
            StopCoroutine(currentLerpCoroutine);
        }

        LerpFOV(hipShotFOV);
        // Start hip-shot animation
        currentLerpCoroutine = StartCoroutine(LerpToPosition(items[itemIndex].transform, hipShotPosition, duration));

    }

    // Lerps the camera position and FOV to the target position and FOV
    private IEnumerator LerpToPosition(Transform objectToMove, Transform destination, float duration)
    {
        Vector3 startPosition = objectToMove.position; // Cache the starting position

        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            objectToMove.position = Vector3.Lerp(startPosition, destination.position, t);
            yield return null;
        }

        // Ensure the object ends up exactly at the end position
        // objectToMove.position = destination.position;
    }

    
    // Method to lerp the FOV of the camera to the target FOV
    private Coroutine fovCoroutine;
    public void LerpFOV(float targetFOV)
    {
        if(fovCoroutine != null)
            StopCoroutine(fovCoroutine); // Stop any ongoing FOV lerping
        
        fovCoroutine = StartCoroutine(LerpFOVCoroutine(targetFOV));
    }

    private IEnumerator LerpFOVCoroutine(float targetFOV)
    {
        float startFOV =  cameraCache.m_Lens.FieldOfView;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            cameraCache.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        // Ensure the FOV ends up exactly at the target FOV
        cameraCache.m_Lens.FieldOfView = targetFOV;
    }

    private void OnDrawGizmos()
    {
        //draw the aim positions
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(aimPosition.position, 0.05f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(hipShotPosition.position, 0.05f);
    }
}