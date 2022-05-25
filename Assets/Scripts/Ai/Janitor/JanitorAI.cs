using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JanitorAI : AICharacter
{
    public Vector3 startPosition;
    public Vector3 endPosition;
    public NavMeshAgent navMeshAgent;

    void Start()
    {
        CharacterStart();
        navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    public override void Seek(Vector3 location)
    {
        navMeshAgent.SetDestination(location);
    }

    public override void Flee(Vector3 location)
    {
    }

    public override void Wander()
    {
        //Debug.Log("en mouvement");
        if (startPosition == Vector3.zero) return;
        if (Vector3.Distance(this.transform.position, startPosition) < 2f)
        {
            navMeshAgent.SetDestination(endPosition);
        }
        if (Vector3.Distance(this.transform.position, endPosition) < 2f)
        {
            navMeshAgent.SetDestination(startPosition);
        }


        //Seek(transform.position == startPosition ? endPosition : startPosition);
    }
}
