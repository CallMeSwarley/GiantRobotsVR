using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomWander : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public bool canWander = true;
    public Animator animator;

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        if (canWander)  // Check if wandering is allowed 
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newDestination = RandomNavMeshLocation(wanderRadius);
                agent.SetDestination(newDestination);
                timer = 0f;
            }
            animator.SetBool("isStopped", false);
        }
        else
        {
            agent.ResetPath();  // Stop movement when canWander is false
            animator.SetBool("isStopped", true);
        }
    }

    Vector3 RandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }
}
