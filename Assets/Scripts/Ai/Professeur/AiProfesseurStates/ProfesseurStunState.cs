using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProfesseurStunState : AiProfesseurState
{
    float timeStun = 0;
    public void Enter(AiProfesseur professeur)
    {
        professeur.navMeshAgentProf.speed = 0f;
        professeur.navMeshAgentProf.isStopped = true;
        professeur.navMeshAgentProf.enabled = false;
        professeur.animator.SetBool("Stun", true);
        professeur.SetSFX("Stun");
        timeStun = 0;
        professeur.sensor.enabled = false;
        professeur.targeting.enabled = false;
        professeur.targetAlarm = null;
    }

    public void Exit(AiProfesseur professeur)
    {
        professeur.animator.SetBool("Stun", false);
        professeur.sensor.enabled = true;
        professeur.targeting.enabled = true;
        professeur.targetAlarm = null;
    }

    public AiProfesseurStateId GetId()
    {
        return AiProfesseurStateId.Stun;
    }

    public void Update(AiProfesseur professeur)
    {
        timeStun += Time.deltaTime;

        if (timeStun >= professeur.config.tempsStun)
        {
            //professeur.SetTrigger("Run");
            professeur.navMeshAgentProf.enabled = true;
            professeur.professeurStateMachine.ChangeState(AiProfesseurStateId.Patrol);
        }

    }
}
