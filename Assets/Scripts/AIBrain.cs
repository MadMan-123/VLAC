using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIBrain : MonoBehaviour
{
    [Header("General")] 
    public Rigidbody Rb;
    public Health Health;  
    [Header("AI")]
    public AIType Type;
    public BehaviourStateManager StateManager;
    public NavMeshAgent Agent;
    public Path path;
    public Node NextNode;
    public float AttackRange;
    public float FieldOfViewAngle = 90f;
    public float ViewDistance = 70f;
    public double IdleTime { get; set; }

    
    public Vector3 LastKnownPlayerPosition { get; set; }

  
    
    //Flags
    [Header("Flags")]
    public bool InvestigateFlag { get; set; } = true;
    public bool ShouldPatrolFlag = true;
    public bool PathFlag = true;
    public bool AttackFlag = true;
    public bool ChaseFlag = true;
    private void Start()
    {
        gameObject.tag = "AI";
        TryGetComponent(out Rb);
        TryGetComponent(out Agent);
        TryGetComponent(out Health);
    }


    public IEnumerator ToggleNavMeshAgentOff(float time)
    {
        //turn of the destination
        Agent.destination = transform.position;
        // Disable isKinematic at the start (optional)
        yield return new WaitForSeconds(time); // Wait for a short duration before re-enabling NavMeshAgent
        // Reset velocity to zero
        Rb.velocity = Vector3.zero;
        //reset the angular velocity
        Rb.angularVelocity = Vector3.zero;

    }
    
    
    private void OnDrawGizmos()
    {
        //draw the attack range
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawWireSphere(position, AttackRange);
        
        
        //draw the target
        Gizmos.color = new Color(15,155,15,1);
        Gizmos.DrawSphere(Agent.destination, 0.5f);
        
        //draw the field of view
        Gizmos.color = Color.grey;
        var forward = transform.forward;
        Gizmos.DrawRay(position, forward * ViewDistance);
        Gizmos.DrawRay(position, Quaternion.Euler(0, FieldOfViewAngle * 0.5f, 0) * forward * ViewDistance);
        Gizmos.DrawRay(position, Quaternion.Euler(0, -FieldOfViewAngle * 0.5f, 0) * forward * ViewDistance);
        
    }

    public void SetDestination(Vector3 pos)
    {
        Agent.destination = pos;
    }

    public void Alert(Transform Source)
    {
        
        LastKnownPlayerPosition = Source.position;
        InvestigateFlag = true; //set the investigate flag to true
        StateManager.ChangeState(AIState.Investigate);
        
    }

}

public struct BehaviourStateManager
{
    public AIState CurrentState { private set; get; }
    public Action AttackBehaviour;
    public Action<AIBrain> ChaseBehaviour;
    public Action IdleBehaviour;
    public Action<AIBrain,List<Node>> PatrolBehaviour;
    public Action<AIBrain> InvestigateBehaviour;
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
    Pathing = 4,
    Investigate = 5
}