using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using Deus;
using Random = UnityEngine.Random;

public class Firearm : Weapon
{
    [SerializeField] private FireArmType fireArmType;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] ParticleSystem muzzleFlash;
    private ObjectPool bulletPool;
    private float timeToCollect;

    [SerializeField] private float recoilForce = 1.0f;
    [SerializeField] private CinemachineImpulseSource impulseSource;


    public float aimFOV = 30f; // Field of view when aiming
    public float hipShotFOV = 100f; // Field of view for hip shot
    private Vector3 bulletFirePos;
    private int activeShotCount = 0;
    private readonly int maxActiveShots = 5; // Maximum number of simultaneous shots

    private void Start()
    {
        timeToCollect = Range / Force;
        bulletPool = new ObjectPool(bulletPrefab, 3, firePoint);
    }


    public override void Use()
    {
        if (CanAttack)
            StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        //flag
        CanAttack = false;

        //raycast to detect if the gun is hitting something
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 0.1f))
        {
            //break the Fire method if the raycast hits anything in a 0.1f distance so the gun doesn't shoot itself
            #if UNITY_EDITOR
            Debug.Log("Gun Is In something");
            #endif
            CanAttack = true;
            yield break;
        }

        //switch behaviour of firearm
        switch (fireArmType)
        {
            case FireArmType.Rifle:
            case FireArmType.Pistol:
                StartCoroutine(FireSingleShot(firePoint.forward * Force));
                break;
            case FireArmType.Blunderbuss:
                SpreadShot();

                break;
        }

        //play the muzzle flash
        muzzleFlash.Play();
    
        //recoil
        ApplyRecoil();
        //play the sound MusketShot
        AudioManager.PlayAudioClip("MusketShot", firePoint.position, 10f);
        //recoil
        yield return new WaitForSeconds(AttackDelay);
        CanAttack = true;
    }
 
    public void ApplyRecoil()
    {
      impulseSource.GenerateImpulseWithVelocity(firePoint.forward * recoilForce);
    }

    private void StopRecoil()
    {
    
    }

    private void SpreadShot(int shotAmount = 5)
    {
        //shoot the ammount of bullets into a random spread
        //create a random spread using the forward direction of the fire point as the base
        for (int i = 0; i < shotAmount; i++)
        {
            // Generate a random direction in a spherical pattern
            Vector3 spreadDirection = firePoint.forward + Random.insideUnitSphere * 0.1f; // Adjust the spread factor (0.1f) as needed

            // Normalize the direction to ensure consistent bullet force
            spreadDirection.Normalize();

            // Fire a bullet using the spread direction
            StartCoroutine(FireSingleShot(spreadDirection * Force));
        }
        
    }

    private GameObject cache;

    // Use this method to fire a single shot
    private IEnumerator FireSingleShot(Vector3 vel)
    {
        activeShotCount++; // Increment active shot count
        // Use the bullet the bullet
        cache = bulletPool.GetObject();
        cache.transform.position = firePoint.position;
        // Add velocity as the forward direction
        if (cache.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = vel;
        }

        if (cache.TryGetComponent<Bullet>(out var bl))
        {
            bl.Pool = bulletPool;
            bl.Damage = Damage;
            bl.Force = Force;
        }

        yield return new WaitForSeconds(0.0005f);

        if (cache.TryGetComponent<TrailRenderer>(out var tr))
        {
            tr.enabled = true;
        }
    
        //raycast to do the damage to the AI
        if (Physics.Raycast(firePoint.position, firePoint.forward, out var hit, Range))
        {
            rb.velocity = Vector3.zero;
            //Pool.ReturnObject(gameObject);
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.TryGetComponent<AIBrain>(out var brain))
            {
                //add on damage Alert to the AI with the source of the bullet being the player
                brain.Alert(transform);
            }
            if (hitObject.TryGetComponent<Health>(out var health))
            {
                health.Damage(Damage);
            }

            if (hitObject.TryGetComponent<Rigidbody>(out var otherRb))
            {
                //calculate the force to apply
                otherRb.AddForce(transform.forward * Force);

                //if AI Brain, stop it
                if (brain != null)
                    StartCoroutine(brain.ToggleNavMeshAgentOff(0.5f));
            }
            
        }
        
        yield return new WaitForSeconds(timeToCollect);
        // Collect the bullet back
        rb.velocity = Vector3.zero;
        tr.enabled = false;
        bulletPool.ReturnObject(cache);

        activeShotCount--; // Decrement active shot count
    }

    private void OnDrawGizmos()
    {
        //draw the fire point
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firePoint.position, 0.05f);
        Gizmos.color = Color.cyan;
        bulletFirePos = firePoint.position + (firePoint.forward / 10);
        Gizmos.DrawWireSphere(bulletFirePos, 0.025f);

        //draw fire direction
        Gizmos.DrawRay(bulletFirePos, firePoint.forward);
    }

    
    
    // Coroutine to wait until the number of active shots decreases
    private IEnumerator WaitForActiveShots()
    {
        while (activeShotCount >= maxActiveShots)
        {
            yield return null;
        }
    }
    
    public enum FireArmType
    {
        Pistol = 0,
        Rifle = 1,
        Blunderbuss = 2
    };
    


    // Override the OnDisable method
    private void OnDisable()
    {
        //hack: set can attack to true
        CanAttack = true;
    }

    private void Update()
    {
    }
}
