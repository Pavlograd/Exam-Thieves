using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LocomotionProfesseur : MonoBehaviour
{
    NavMeshAgent professeur;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        professeur = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (professeur.hasPath)
        {
            animator.SetFloat("Speed", professeur.velocity.magnitude);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }
}
