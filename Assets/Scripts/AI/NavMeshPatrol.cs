using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public class NavMeshPatrol : MonoBehaviour
{
    
    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
    
    #region Inspector

    [SerializeField] private Animator animator;

    [Header("Waypoints")]
    [SerializeField] private bool randomOrder;
    
    [SerializeField] private List<Transform> waypoints;

    [SerializeField] private bool waitAtWaypoit = true;
    
    [Min(0)]
    [Tooltip("Min/Max wait duration a each waypoint in seconds. WaitAtWaypoints needs to be enabled.")]
    [SerializeField] private Vector2 waitDuration = new Vector2(1, 5);

    #endregion

    private NavMeshAgent navMeshAgent;

    private int currentWaypoitIndex = -1;

    private bool waiting;

    #region Unity Event Functions

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = waitAtWaypoit;
    }

    private void Start()
    {
        SetNextWaypoit();
    }

    private void Update()
    {
        animator.SetFloat(MovementSpeed, navMeshAgent.velocity.magnitude);
        if (!navMeshAgent.isStopped)
        {
            CheckIfWaypoitIsReached();
        }
    }

    #endregion

    #region Navigation

    public void StopPatrolForDialogue()
    {
        StopPatrol();
        //Subscribe to DialogueClosed to resume patrol.
        DialogueController.DialogueClosed += ResumePatrol;
    }

    public void StopPatrol()
    {
        navMeshAgent.isStopped = true;
    }

    public void ResumePatrol()
    {
        navMeshAgent.isStopped = false;
        //Unsubscribe from dialogue once triggered. Will do nothing if never subscribed
        DialogueController.DialogueClosed -= ResumePatrol;
    }

    private void SetNextWaypoit()
    {
        switch (waypoints.Count)
        {
            case 0:
                Debug.LogError("No waypoints set for NavMeshPatrol", this);
                return;
            case 1:
                if (randomOrder)
                {
                    Debug.LogError("Only one waypoint set for NavMeshPatrol. Need at least 2 with randomOrder enabled.", this);
                    return;
                }
                else
                {
                    Debug.LogWarning("Only one waypoint set for NavMeshPatrol", this);
                        break;
                }
        }
        
        if (randomOrder)
        {
            int newWaypointIndex;
            // Pick a new random Waypoint Index until it is different from the current one.
            do
            {
                newWaypointIndex = Random.Range(0, waypoints.Count);
            }
            while (newWaypointIndex == currentWaypoitIndex);
            currentWaypoitIndex = newWaypointIndex;


        }
        else
        {
            // Increase waypoit index and loop back around to 0.
            currentWaypoitIndex = (currentWaypoitIndex + 1) % waypoints.Count;
        }
        
        // Set the destination of the navMeshAgent based on the current waypoit index
        // navMeshAgent automatically tries to reach the destination
        navMeshAgent.destination = waypoints[currentWaypoitIndex].position;
    }

    private void CheckIfWaypoitIsReached()
    {
        if (waiting) { return; }
        
        //Abort if still calculating path to destination
        if (navMeshAgent.pathPending) { return; }

        //Has NavMeshAgent reached it's destination?
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.01f) // small addition because of floating poit errors
        {
            if (waitAtWaypoit)
            {
                StartCoroutine(WaitBeforeNextWaypoit(Random.Range(waitDuration.x, waitDuration.y)));
            }
            else
            {
                SetNextWaypoit(); 
            }
        }
    }

    private IEnumerator WaitBeforeNextWaypoit(float duration)
    {
        waiting = true;
        yield return new WaitForSeconds(duration);
        SetNextWaypoit();
        waiting = false;
    }
    
    #endregion
}
