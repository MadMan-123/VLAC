using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class Node : MonoBehaviour
{
    public Node Next;
    bool bUpdate = false;
    private void Update()
    {
        
        //align the node to the ground
        RaycastHit hit;
        if ( !bUpdate && Physics.Raycast(transform.position, Vector3.down, out hit, 10))
        {
            transform.position = hit.point;
            bUpdate = true;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;

        }
        //draw a line to the next node
        if (Next)
        {
            Gizmos.DrawSphere(transform.position, 0.5f);
            Gizmos.DrawLine(transform.position, Next.transform.position);

        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
    private void OnDrawGizmosSelected()
    {
        bUpdate = false;
        Gizmos.color = Color.cyan;
        
        
       
    }
}
