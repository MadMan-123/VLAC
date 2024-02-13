using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIBrain : MonoBehaviour
{
    public AIType Type;
    public float AttackRange;
    public float ChaseRange;
    public NavMeshAgent Agent;
    public BehaviourStateManager StateManager;
    public Path path;
    public double IdleTime { get; set; }
    
    public bool ShouldPatrolFlag = true;
    public bool PathFlag = true;
    public bool AttackFlag = true;
    
    public Node NextNode;

    public float FieldOfViewAngle = 90f;
    public float ViewDistance = 20f;
    
    private void Start()
    {
        gameObject.tag = "AI";
        Agent = GetComponent<NavMeshAgent>();
    }


    
    
    private void OnDrawGizmos()
    {
        //draw the attack range
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawWireSphere(position, AttackRange);
        //draw the chase range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(position, ChaseRange);
        
        //draw the target
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Agent.destination, 0.5f);
        
        //draw the field of view
        Gizmos.color = Color.yellow;
        var forward = transform.forward;
        Gizmos.DrawRay(position, forward * ViewDistance);
        Gizmos.DrawRay(position, Quaternion.Euler(0, FieldOfViewAngle * 0.5f, 0) * forward * ViewDistance);
        Gizmos.DrawRay(position, Quaternion.Euler(0, -FieldOfViewAngle * 0.5f, 0) * forward * ViewDistance);
        
    }

    public void SetDestination(Vector3 pos)
    {
        Agent.destination = pos;
    }
}

public struct BehaviourStateManager
{
    public AIState CurrentState { private set; get; }
    public Action AttackBehaviour;
    public Action<AIBrain> ChaseBehaviour;
    public Action IdleBehaviour;
    public Action<AIBrain,List<Node>> PatrolBehaviour;
    
    public Action<AIBrain,Path> PathFollowBehaviour;
    
    public void ChangeState(AIState newState)
    {
        //make the state change
        CurrentState = newState;
        //change the behaviour
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
    Pathing = 4
}