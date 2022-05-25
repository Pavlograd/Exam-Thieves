using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFindTargetState : AiState
{
    bool vigilance = false;
    bool vu = false;

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.speed = agent.config.vitesseMarche * DifficultySettings.datas.speedGuardsMult;

        agent.SetSFX("Walk");
        agent.SetVoice("Whistle");
        vigilance = false;
        vu = false;
    }

    public void Exit(AiAgent agent)
    {
        //agent.iconAlertness.gameObject.SetActive(false);
    }

    public AiStateId GetId()
    {
        return AiStateId.FindTarget;
    }

    public void Update(AiAgent agent)
    {
        //agent.animator.SetTrigger("RetourNormal");
        if (agent.navMeshAgent.enabled && !agent.navMeshAgent.hasPath)
        {
            agent.character.Wander();
        }
        if (agent.alarme)
        {
            Debug.Log("ALARME ALARME");
            agent.stateMachine.ChangeState(AiStateId.Alarm);
        }
        if (agent.targeting.HasTarget)
        {
            if (Vector3.Distance(agent.targeting.Target.transform.position, agent.transform.position) > (DifficultySettings.datas.FOVGuards * 0.75))
            {
                if (!vigilance && agent.navMeshAgent.enabled)
                {
                    agent.navMeshAgent.SetDestination(agent.targeting.Target.transform.position);
                    agent.SetSFX("Idle");
                    agent.SetVoice("Ah");
                    agent.navMeshAgent.speed = (agent.config.vitesseMarche - 1.5f) * DifficultySettings.datas.speedGuardsMult;

                    vigilance = true;
                    agent.iconAlertness.gameObject.SetActive(true);
                }
                //Debug.Log
            }
            else
            {
                /* relativePos = agent.targetCatch.position - agent.transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                agent.transform.rotation = rotation;*/
                //agent.navMeshAgent.isStopped = true;
                //

                //
                if (!vu)
                {
                    agent.navMeshAgent.enabled = false;

                    agent.SetSFX("Idle");
                    agent.SetVoice("Ah");
                    vu = true;
                    agent.iconAlertness.gameObject.SetActive(false);
                    agent.iconChase.gameObject.SetActive(true);
                    agent.transform.LookAt(agent.targeting.Target.transform);///
                    agent.StartCoroutine(agent.ChangeToChaseMod());
                }
            }
        }
        else
        {
            //agent.animator.SetTrigger("RetourNormal");
            //agent.iconAlertness.gameObject.SetActive(false);
            //agent.navMeshAgent.speed = agent.config.vitesseMarche;

            if (vu || vigilance)
            {

                agent.SetSFX("Walk");
                agent.SetVoice("Whistle");
                agent.iconChase.gameObject.SetActive(false);
                agent.iconAlertness.gameObject.SetActive(false);
                agent.navMeshAgent.speed = agent.config.vitesseMarche * DifficultySettings.datas.speedGuardsMult;
                vigilance = false;
                vu = false;
            }
        }
    }
}
