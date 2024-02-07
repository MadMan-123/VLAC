using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firearm : Weapon
{
    [SerializeField] Transform FirePoint;
    public GameObject BulletPrefab;
    public ParticleSystem MuzzleFlash;
    public ParticleSystem HitEffect;
    
    private void Start()
    {
        Force = 10;
        AttackDelay = 0.5f;
        Range = 100f;
        Damage = 10;
        type = WeaponType.FIREARM;
    }

    protected override void OnAttack()
    {
        if(CanAttack)
            StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        CanAttack = false;
        
        //raycast to get the hit point
        RaycastHit hit;
        if (Physics.Raycast(FirePoint.position, FirePoint.forward, out hit, Range))
        {
            //MuzzleFlash.Play();
            Debug.Log(hit.transform.name);
        }

        yield return new WaitForSeconds(AttackDelay);
        CanAttack = true;
    }

    private void OnDrawGizmos()
    {
        //draw the fire point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(FirePoint.position, 0.05f);
    }
}
