using System.Collections;
using System.Collections.Generic;
using Deus;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public ObjectPool Pool;
    void Start()
    {
        TryGetComponent<Rigidbody>(out rb);
        
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        rb.velocity = Vector3.zero;
        //Pool.ReturnObject(gameObject);
    }
}
