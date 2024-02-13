using System.Collections;
using System.Collections.Generic;
using Deus;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public ObjectPool Pool;
    public int Damage;
    void Start()
    {
        TryGetComponent<Rigidbody>(out rb);
        
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        rb.velocity = Vector3.zero;
        //Pool.ReturnObject(gameObject);

        if (other.TryGetComponent<Health>(out var health))
        {
            health.Damage(Damage);
        }
        
        if(other.TryGetComponent<Rigidbody>(out var otherRb))
        {
            //calculate the force to apply
            otherRb.AddForceAtPosition(transform.forward * rb.velocity.magnitude, transform.position);
        }
    }
}
