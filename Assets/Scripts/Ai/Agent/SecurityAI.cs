using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecurityAI : AICharacter
{
    void Start()
    {
        CharacterStart();
    }

    public override void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    public override void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }


    /* public void Pursue()
     {
         Vector3 targetDir = target.transform.position - this.transform.position;

         float relativeHeading = Vector3.Angle(this.transform.forward, this.transform.TransformVector(target.transform.forward));
         float toTarget = Vector3.Angle(this.transform.forward, this.transform.TransformVector(targetDir));

         if ((toTarget > 90 && relativeHeading < 20) || target.GetComponent<PlayerController>().moveSpeed < 0.01f) //0.01f pour eviter erreurn floating point
         {
             Seek(target.transform.position);
             return;
         }

         float lookAhead = targetDir.magnitude / (agent.speed + target.GetComponent<PlayerController>().moveSpeed);
         Seek(target.transform.position + target.transform.forward * lookAhead);
     }*/

    public override void Wander()
    {
        Vector3 wanderTarget = new Vector3(size, 0.0f, size);
        float wanderJitter = size;

        //RANDOM
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter);

        NavMeshHit hit;

        // size / 5 is not permanent will change later
        if (NavMesh.SamplePosition(wanderTarget, out hit, size / 5, NavMesh.AllAreas) && (int)hit.position.y == 0)
        {
            Seek(hit.position);
        }
    }
}