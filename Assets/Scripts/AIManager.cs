using System;
using System.Collections;
using System.Collections.Generic;
using Deus;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public struct AIGroupDef
{
    public GameObject Prefab;
    public int count;
    public List<Node> Nodes;
    public Path path;
}

public class AIManager : MonoBehaviour
{
    [SerializeField] private AIGroupDef[] AIPrefabs = new AIGroupDef[PrefabCount];


    private const int PrefabCount = 5;
    private readonly ObjectPool[] aiPools = new ObjectPool[5];
    
    List<AIBrain> aiBrainsBuffer = new List<AIBrain>();
    List<Node> nodes = new List<Node>();
    void Start()
    {
        for (int i = 0; i < PrefabCount; i++)
        {
            if (AIPrefabs[i].Prefab == null || AIPrefabs == null)
                continue;
            aiPools[i] = new ObjectPool(AIPrefabs[i].Prefab, AIPrefabs[i].count, transform);
            //null checks abound
            aiPools[i].Dynamic = false;
            aiPools[i].SetActiveOnGet = false;
            //get the brains
            for (int j = 0; j < aiPools[i].Count; j++)
            {
                List<GameObject> buffer = aiPools[i].GetAllObjects();
                GameObject ai = aiPools[i].GetObject();

                foreach (var o in buffer)
                {
                    if (o.TryGetComponent(out AIBrain brain))
                    {
                        aiBrainsBuffer.Add(brain);
                    }
                }
  
                //lambda function to add the nodes
                AIPrefabs[i].Nodes.ForEach(node =>
                {
                    nodes.Add(node);
                });
                
            }
            
        }
        
        SpawnInitialAI();
    }
    private void SpawnInitialAI()
    {
        for(int i = 0; i < aiPools.Length; i++)
        { 
            var pool = aiPools[i];   
            if(pool == null)
                continue;


            for(int j = 0; j < pool.Count; j++)
            { 
                GameObject ai = pool.GetObject();
                if(ai == null)
                    break;

                if (ai.TryGetComponent(out AIBrain brain))
                {
                    ResetHealth(brain);

                    // If there is no path, give the AI Patrol behavior
                    if (AIPrefabs[i].path == null)
                    {
                        if (AIPrefabs[i].Nodes.Count == 0)
                            continue;
                        var node = AIPrefabs[i].Nodes[UnityEngine.Random.Range(0, AIPrefabs[i].Nodes.Count)];
                        brain.StateManager.PatrolBehaviour += Patrol;
                        SetToNavmesh(ai.transform, node.transform.position);
                        brain.StateManager.ChangeState(AIState.Patrol);
                    }
                    // If there is a path, give the AI FollowPath behavior
                    else
                    {
                        brain.path = AIPrefabs[i].path;
                        brain.NextNode = AIPrefabs[i].path.nodes[0];
                        brain.StateManager.PathFollowBehaviour += FollowPath;
                        SetToNavmesh(ai.transform, brain.NextNode.transform.position);
                        
                        brain.StateManager.ChangeState(AIState.Pathing);
                    }  
                    
                }


                ai.SetActive(true);
                brain.StateManager.ChaseBehaviour += Chase;
                brain.StateManager.InvestigateBehaviour += Investigate;
                
            }
        }

    }
    bool SetToNavmesh(Transform transform,Vector3 position)
    {
        NavMeshHit hit;
        bool cache = NavMesh.SamplePosition(position, out hit, 1f, NavMesh.AllAreas);
        transform.position = hit.position;
        return cache;
    }
    Node GetRandomNode(List<Node> nodes)
    {
        if (nodes == null)
            return null;
        return nodes[UnityEngine.Random.Range(0, nodes.Count - 1)];
    }
    bool IsNodeOccupied(Node node)
    {
        Collider[] colliders = Physics.OverlapSphere(node.transform.position, 1);
        foreach (var col in colliders)
        {
            if (col.CompareTag("AI"))
            {
                return true;
            }
        }

        return false;
    }
    private void Patrol(AIBrain brain,List<Node> nodes)
    {
        if (brain.ShouldPatrolFlag)
        {
            brain.NextNode = GetRandomNode(nodes);
            brain.ShouldPatrolFlag = false;
            //make sure the point is on the navmesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition( brain.NextNode.transform.position, out hit, 1f, NavMesh.AllAreas)) {
                // Sample position successful, set it as the destination
                brain.SetDestination(hit.position);
            } else {
                // Sample position failed, handle the error
                // For example, you can log a message or use a fallback position
                Debug.LogWarning("Failed to sample position from NavMesh.");
            }
        }
       
            
        
        //using sqrMagnitude to avoid the expensive square root operation
        if(brain.Agent.remainingDistance < 1.5f)
        {
            //set the next node as the target
            brain.ShouldPatrolFlag = true;
        }
    }
    public void UpdateBehaviour(AIBrain brain)
    {
        AIState CurrentState = brain.StateManager.CurrentState;
        BehaviourStateManager StateManager = brain.StateManager;
        switch (CurrentState)
        {
            case AIState.Attack:
                //check if attack behaviour is null
                if (StateManager.AttackBehaviour == null)
                {
                    //if it is null, set the state to idle
                    StateManager.ChangeState(AIState.Idle);
                    Debug.LogWarning("Attack Behaviour is null, changing state to idle.");
                    return;
                }
                //do attack behaviour
                StateManager.AttackBehaviour?.Invoke();
                break;
            case AIState.Chase:
                if (StateManager.ChaseBehaviour == null)
                {
                    //if it is null, set the state to idle
                    StateManager.ChangeState(AIState.Idle);
                    Debug.LogWarning("Chase Behaviour is null, changing state to idle.");

                    return;
                }
                //chase behaviour
                StateManager.ChaseBehaviour?.Invoke(brain);
                break;
            case AIState.Idle:
                //idle behaviour
                IdleBehaviour(brain);
                break;
            case AIState.Patrol:
                if (StateManager.PatrolBehaviour == null)
                {
                    //if it is null, set the state to idle
                    StateManager.ChangeState(AIState.Idle);
                    Debug.LogWarning("Patrol Behaviour is null, changing state to idle.");

                    return;
                }
                //patrol behaviour
                StateManager.PatrolBehaviour?.Invoke(brain,nodes);
                break;
            case AIState.Pathing:
                if (StateManager.PathFollowBehaviour == null)
                {
                    //if it is null, set the state to idle
                    StateManager.ChangeState(AIState.Idle);
                    Debug.LogWarning("Path Behaviour is null, changing state to idle.");

                    return;
                }
                //patrol behaviour
                StateManager.PathFollowBehaviour?.Invoke(brain,brain.path);
                break;
            case AIState.Investigate:
                if (StateManager.InvestigateBehaviour == null)
                {
                    //if it is null, set the state to idle
                    StateManager.ChangeState(AIState.Idle);
                    Debug.LogWarning("Investigate Behaviour is null, changing state to idle.");

                    return;
                }
                //investigate behaviour
                StateManager.InvestigateBehaviour?.Invoke(brain);
                break;
        }
    }
    private void IdleBehaviour(AIBrain brain)
    {
        //look around for 10 seconds
        if (brain.IdleTime < 10)
        {
            brain.IdleTime += Time.deltaTime;
            brain.transform.Rotate(0, 1, 0);
        }
        else
        {
            //reset the idle time
            brain.IdleTime = 0;
            //set the state to patrol
            brain.StateManager.ChangeState(AIState.Patrol);
        }
    }
    private void FollowPath(AIBrain brain, Path path)
    {
        if (brain.PathFlag)
        {
            brain.NextNode = brain.NextNode.Next;
            //sample the next node
            NavMeshHit hit;
            NavMesh.SamplePosition(
                brain.NextNode.transform.position,
                out hit,
                1f,
                NavMesh.AllAreas
            );
            brain.SetDestination(hit.position);
            brain.PathFlag = false;
        }

        
        if(brain.Agent.remainingDistance < 1.5f)
        {
            brain.PathFlag = true;
        }
        
    }

    float sqrDistance(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }
    
    private void Update()
    {
        //update the AI behaviours
        for(int i = 0; i < aiBrainsBuffer.Count; i++)
        {
            //check if the AI is active
            //check brain is active
            if(CheckIfBrainIsActive(aiBrainsBuffer[i]))
                return;
            
            //check if the player is in sight
            if (CheckForPlayer(aiBrainsBuffer[i]))
            {
                aiBrainsBuffer[i].LastKnownPlayerPosition = Player.Instance.transform.position;
            }
            else if ( aiBrainsBuffer[i].StateManager.CurrentState == AIState.Chase)
            {
                aiBrainsBuffer[i].StateManager.ChangeState(AIState.Investigate);
                aiBrainsBuffer[i].InvestigateFlag = true;
            }
            UpdateBehaviour(aiBrainsBuffer[i]);

        }
    }
    
    public void ResetHealth(AIBrain brain)
    {
        brain.Health.SetCurrentHealth(brain.Health.MaxHealth);
    }
    
    private bool CheckForPlayer(AIBrain brain)
    {
        // Calculate direction to the player
        var transform1 = brain.transform;
        Vector3 directionToPlayer = Player.Instance.transform.position - transform1.position;
        float angleToPlayer = Vector3.Angle(directionToPlayer, transform1.forward);

        // Check if the player is within the field of view angle
        if (angleToPlayer < brain.FieldOfViewAngle * 0.5f)
        {
            // Raycast to check if there are any obstacles between the AI and the player
            RaycastHit hit;
            if (Physics.Raycast(transform1.position, directionToPlayer, out hit,brain.ViewDistance))
            {

                if (hit.collider.CompareTag("Player"))
                {
                    // Player is in sight
                    brain.StateManager.ChangeState(AIState.Chase);
                    
                    return true;
                }
            }
        }

        // Player is not in sight
        return false;
    }
    
    void Chase(AIBrain brain)
    {
        //check brain is active
        if(CheckIfBrainIsActive(brain))
            return;
        // Set the destination to the player's position
        brain.SetDestination(Player.Instance.transform.position);
      
        
        if(brain.Agent.remainingDistance < brain.AttackRange)
        {
            // Attack the player
            brain.StateManager.ChangeState(AIState.Attack);
        }
    }
    
    void Investigate(AIBrain brain)
    {
         //check brain is active
        if(CheckIfBrainIsActive(brain))
            return;
        if(brain.InvestigateFlag)
        {
            // Set the destination to the last known position of the player
            brain.SetDestination(brain.LastKnownPlayerPosition);
            brain.InvestigateFlag = false;
            brain.IdleTime = 0;
        }

        if (brain.Agent.remainingDistance < 1.5f)
        {
            //look around for 5 seconds
            //turn the AI around
            brain.transform.Rotate(0, 1, 0);    
            //idle for 5 seconds
            if (brain.IdleTime < 5)
            {
                brain.IdleTime += Time.deltaTime;
            }
            else
            {
                //reset the idle time
                brain.IdleTime = 0;
                
                //if there isnt a path, set the state to patrol
                if (brain.path)
                {
                    //set the state to patrol
                    brain.StateManager.ChangeState(AIState.Pathing);
                }
                else
                {
                    brain.StateManager.ChangeState(AIState.Patrol);
                }

            }
        }
    }
    
    void ReturnToPool(AIBrain brain)
    {
        //return the AI to the pool
        brain.gameObject.SetActive(false);
        //reset the AI
        brain.StateManager.ChangeState(AIState.Idle);
        
    }
    
    bool CheckIfBrainIsActive(AIBrain brain)
    {
        return brain.gameObject.activeSelf;
    }
}



