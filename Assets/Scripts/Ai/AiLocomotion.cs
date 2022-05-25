using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiLocomotion : MonoBehaviour
{
    

    NavMeshAgent agent;
    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        float velocity = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

       if (agent.hasPath)
        {
            
            animator.SetFloat("Speed", agent.speed);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }
}
