using System.Collections;
using System.Collections.Generic;
using Deus;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public ObjectPool Pool;
    public int Damage;
    public float Force;
    public Transform Source;
    void Start()
    {
        TryGetComponent<Rigidbody>(out rb);
        
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //if the bullet hits an AI
        
        //return the bullet to the pool
        Pool.ReturnObject(gameObject);
    }

    
    
}
