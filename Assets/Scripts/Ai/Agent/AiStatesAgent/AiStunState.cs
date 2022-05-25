using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiStunState : AiState
{
    float timeStun = 0;
    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = 0f;
        Debug.Log("Fuck I'm stun");
        //agent.navMeshAgent.velocity = Vector3.zero;
        //agent.navMeshAgent.isStopped = true;
        agent.navMeshAgent.enabled = false;
        //agent.ResetAllTriggers();
        
        agent.SetSFX("Stun");
        timeStun = 0;
        agent.sensor.enabled = false;
        agent.targeting.enabled = false;
        agent.animator.SetBool("Stun", true);
    }

    public void Exit(AiAgent agent)
    {
        Debug.Log("I'm not stun anymore");
        agent.sensor.enabled = true;
        agent.targeting.enabled = true;
        agent.animator.SetBool("Stun", false);
    }

    public AiStateId GetId()
    {
        return AiStateId.Stun;
    }

    public void Update(AiAgent agent)
    {
        timeStun += Time.deltaTime;

        if (timeStun >= agent.config.tempsStun)
        {
            //agent.SetTrigger("Run");
            agent.navMeshAgent.enabled = true;
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
        }
    }
}