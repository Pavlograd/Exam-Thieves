using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCaptureState : AiState
{
    public void Enter(AiAgent agent)
    {
        agent.animator.SetBool("Catch", true);

        //agent.head;
        if (agent.targetCatch != null)
        {
            Vector3 relativePos = agent.targetCatch.position - agent.transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            agent.transform.rotation = rotation;
        }

        //agent.navMeshAgent.isStopped = true;
        agent.navMeshAgent.enabled = false;

        agent.SetSFX("Idle");
        agent.SetVoice("Ah");
    }

    public void Exit(AiAgent agent)
    {
        //agent.animator.ResetTrigger("Fall");
        //agent.animator.ResetTrigger("Catch");

        //agent.ResetAllTriggers();
        agent.targetCatch = null;
        agent.navMeshAgent.enabled = true;

    }

    public AiStateId GetId()
    {
        return AiStateId.Capture;
    }

    public void Update(AiAgent agent)
    {


    }
}
