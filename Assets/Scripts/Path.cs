using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Path : MonoBehaviour
{
    public Color lineColor;
    public List<Node> nodes = new List<Node>();

    private bool update = false;
    
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        if(!update)
        {
            // Draw lines between nodes
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                nodes[i].Next = nodes[i + 1];
            }
        
            //make the last node point to the first node
            nodes[^1].Next = nodes[0];
            update = true;
        }

        
    }

}
