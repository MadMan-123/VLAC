using System.Collections;
using System.Collections.Generic;
using Deus;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    static readonly int prefabCount = 5;
    private ObjectPool[] AIPools = new ObjectPool[prefabCount];
    [SerializeField] private static GameObject[] AIPrefabs = new GameObject[prefabCount];
    void Start()
    {
        for(int i = 0; i < prefabCount; i++)
        {
            AIPools[i] = new ObjectPool(AIPrefabs[i], 5, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < prefabCount; i++)
        {
            // give the AI a behaviour to follow
            
        }
    }
}


public enum AIType
{
    Private = 0,
    Grenadier = 1,
    Cossack = 2,
    Napoleon = 3
}
public enum AIState
{
    Idle = 0,
    Patrol = 1,
    Chase = 2,
    Attack = 3,
    Dead = 4
}