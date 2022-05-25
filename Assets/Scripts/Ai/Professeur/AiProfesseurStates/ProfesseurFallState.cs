using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfesseurFallState : AiProfesseurState
{
    float timeFall = 0;
    public void Enter(AiProfesseur professeur)
    {
        professeur.navMeshAgentProf.speed = 0f;
        timeFall = 0;
        professeur.navMeshAgentProf.isStopped = true;
        professeur.navMeshAgentProf.enabled = false;
        professeur.animator.SetBool("Fall", true);
        professeur.SetSFX("Fall");
        professeur.sensor.enabled = false;
        professeur.targeting.enabled = false;
        
    }

    public void Exit(AiProfesseur professeur)
    {
        professeur.sensor.enabled = true;
        professeur.targeting.enabled = true;
        professeur.targetAlarm = null;
        professeur.animator.SetBool("Fall", false);
    }

    public AiProfesseurStateId GetId()
    {
        return AiProfesseurStateId.FallPaint;
    }

    public void Update(AiProfesseur professeur)
    {
        timeFall += Time.deltaTime;

        if (timeFall > professeur.config.tempsFall)
        {
            //professeur.animator.ResetTrigger("Fall");
            professeur.navMeshAgentProf.enabled = true;
            professeur.professeurStateMachine.ChangeState(AiProfesseurStateId.Patrol);
        }
    }
}
