using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFallPaintState : AiState
{
    float timeFall = 0;

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = 0f;
        timeFall = 0;
        //agent.navMeshAgent.isStopped = true;
        agent.navMeshAgent.enabled = false;
       
        agent.SetSFX("Fall");
        agent.sensor.enabled = false;
        agent.targeting.enabled = false;
        Debug.Log("aiolle");
        agent.animator.SetBool("Fall", true);
    }

    public void Exit(AiAgent agent)
    {
        agent.sensor.enabled = true;
        agent.targeting.enabled = true;
       
        agent.animator.SetBool("Fall", false);
    }

    public AiStateId GetId()
    {
        return AiStateId.FallPaint;
    }

    public void Update(AiAgent agent)
    {
        agent.StartCoroutine(agent.ChangeStopCatch());
        timeFall += Time.deltaTime;

        if (timeFall > agent.config.tempsFall)
        {
            
            
            agent.navMeshAgent.enabled = true;
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
        }
    }
}
