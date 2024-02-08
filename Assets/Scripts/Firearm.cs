using System.Collections;
using UnityEngine;
using Cinemachine;
using Deus;

public class Firearm : Weapon
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] private FireArmType fireArmType;
    private ObjectPool bulletPool;
    private float timeToCollect;

    public CinemachineVirtualCamera virtualCamera;
    public float recoilAmount = 1f;
    public float recoilSpeed = 1f;
    public Transform aimPosition; // Position to aim at
    public Transform hipShotPosition; // Position for hip shot
    public float aimFOV = 30f; // Field of view when aiming
    public float hipShotFOV = 60f; // Field of view for hip shot
    public float aimSpeed = 5f; // Speed of lerping to aim position
    public float hipShotSpeed = 5f; // Speed of lerping to hip shot position
    private Vector3 originalPosition;
    private Vector3 BulletFirePos;

    private void Start()
    {
        timeToCollect = Range / Force;
        bulletPool = new ObjectPool(bulletPrefab, 3, firePoint);
        originalPosition = virtualCamera.transform.localPosition;
    }

    private IEnumerator RecoilCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < recoilSpeed)
        {
            // Calculate recoil position based on elapsed time and recoil amount
            float recoilFraction = Mathf.SmoothStep(0f, 1f, elapsed / recoilSpeed);
            Vector3 recoilOffset = Vector3.up * recoilAmount * recoilFraction;

            // Apply recoil to camera position
            virtualCamera.transform.localPosition = originalPosition + recoilOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset camera position after recoil
        virtualCamera.transform.localPosition = originalPosition;
    }

    public override void Use()
    {
        if(CanAttack)
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
            yield break;
        }
        
        //switch behaviour of firearm
        switch (fireArmType)
        {
            case FireArmType.Rifle:
            case FireArmType.Pistol:
                StartCoroutine(NormalShot());
                break;
            case FireArmType.Blunderbuss:
                SpreadShot();

                break;
        }
        //play the muzzle flash
        muzzleFlash.Play();
        //recoil
        StartCoroutine(RecoilCoroutine());
        yield return new WaitForSeconds(AttackDelay);
        CanAttack = true;
    }

    private void SpreadShot()
    {
        
    }

    private GameObject cache;
    private IEnumerator NormalShot()
    {
        //use the bullet the bullet
        cache = bulletPool.GetObject(firePoint);
        //add velocity as the forward direction
        Vector3 vel = firePoint.forward * Force;

        if (cache.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity += vel;
        }

        if (cache.TryGetComponent<Bullet>(out var bl))
        {
            bl.Pool = bulletPool;
        }

        yield return new WaitForSeconds(0.001f);

        if (cache.TryGetComponent<TrailRenderer>(out var tr))
        {
            tr.enabled = true;
        }

        yield return new WaitForSeconds(timeToCollect);
        //collect the bullet back
        rb.velocity = Vector3.zero;

        tr.enabled = false;
        bulletPool.ReturnObject(cache);
    }

    private void OnDrawGizmos()
    {
        //draw the fire point
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firePoint.position, 0.05f);
        Gizmos.color = Color.cyan;
        BulletFirePos = firePoint.position + (firePoint.forward / 10);
        Gizmos.DrawWireSphere(BulletFirePos, 0.025f);
        
        //draw fire direction
        Gizmos.DrawRay(BulletFirePos, firePoint.forward);
    }

    // Method to aim the firearm
    public void Aim()
    {
        StartCoroutine(LerpToPosition(aimPosition.position, aimFOV, aimSpeed));
    }

    // Method for hip shot
    public void HipShot()
    {
        StartCoroutine(LerpToPosition(hipShotPosition.position, hipShotFOV, hipShotSpeed));
    }

    // Lerps the camera position and FOV to the target position and FOV
    private IEnumerator LerpToPosition(Vector3 targetPosition, float targetFOV, float speed)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = virtualCamera.transform.localPosition;
        

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * speed;
            virtualCamera.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            //virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime);
            yield return null;
        }
    }
}

public enum FireArmType
{
    Pistol = 0,
    Rifle = 1,
    Blunderbuss = 2
}
