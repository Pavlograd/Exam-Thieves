using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAlarmState : AiState
{
    public GameObject target;
    public bool arrive;
    public void Enter(AiAgent agent)
    {
        Debug.Log("Enter State Alarm");
        target = agent.targetAlarme;
        agent.navMeshAgent.speed = agent.config.vitesseCourse * DifficultySettings.datas.speedGuardsMult;
        agent.iconAlarm.gameObject.SetActive(true);

        agent.SetSFX("Run");
        agent.character.Seek(target.transform.position);
        arrive = false;
    }

    public void Exit(AiAgent agent)
    {
        Debug.Log("Exit State ALARM");
        target = null;
        agent.iconAlarm.gameObject.SetActive(false);
        agent.alarme = false;
        // agent.animator.SetTrigger("RetourNormal");
        arrive = false;
    }

    public AiStateId GetId()
    {
        return AiStateId.Alarm;
    }

    public void Update(AiAgent agent)
    {
        if (!agent.alarme || target == null)
        {
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
        }
        if (agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
        }
        if (target != null && Vector3.Distance(agent.transform.position, target.transform.position) < 4f)
        {
            arrive = true;
            Debug.Log("il y a un probleme");
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
        }
    }
}
