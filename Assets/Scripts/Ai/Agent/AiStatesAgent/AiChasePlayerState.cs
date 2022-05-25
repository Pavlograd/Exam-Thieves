using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChasePlayerState : AiState
{
    float timer = 0.0f;

    public void Enter(AiAgent agent)
    {
        agent.iconChase.gameObject.SetActive(true);

        agent.SetSFX("Run");
        agent.navMeshAgent.speed = agent.config.vitesseCourse * DifficultySettings.datas.speedGuardsMult;
        agent.navMeshAgent.stoppingDistance = 1.5f;

        MusicManager musicManager = GameObject.Find("Music").GetComponent<MusicManager>();

        if (agent.targeting.HasTarget && agent.targeting.Target.GetComponent<PlayerSetup>().isLocalPlayer) musicManager.SwitchToPursuit();
    }

    public void Exit(AiAgent agent)
    {
        agent.iconChase.gameObject.SetActive(false);
        //agent.ResetAllTriggers();
        agent.navMeshAgent.stoppingDistance = 0.0f;

        MusicManager musicManager = GameObject.Find("Music").GetComponent<MusicManager>();

        musicManager.SwitchToLow();
    }

    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Update(AiAgent agent)
    {
        if (!agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
            return;
        }
        if (!agent.enabled)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (!agent.navMeshAgent.hasPath && agent.navMeshAgent.enabled)
        {
            agent.navMeshAgent.destination = agent.targeting.TargetPosition;
        }

        if (timer < 0.0f)
        {
            float sqdistance = (agent.targeting.TargetPosition - agent.navMeshAgent.destination).sqrMagnitude;
            if (sqdistance > agent.config.maxDistance * agent.config.maxDistance && agent.navMeshAgent.enabled)
            {
                agent.navMeshAgent.destination = agent.targeting.TargetPosition;
            }
            timer = agent.config.maxTime;
        }
        //agent.navMeshAgent.destination = agent.targeting.TargetPosition;
    }
}