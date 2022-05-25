using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfesseurAlarmState : AiProfesseurState
{
    public void Enter(AiProfesseur professeur)
    {
        professeur.navMeshAgentProf.speed = 2.5f;
        professeur.Seek(professeur.currentWaypoint.position);
        professeur.SetSFX("Run");
        professeur.iconAlarm.gameObject.SetActive(true);
    }

    public void Exit(AiProfesseur professeur)
    {
        professeur.iconAlarm.gameObject.SetActive(false);
    }

    public AiProfesseurStateId GetId()
    {
        return AiProfesseurStateId.Alarm;

    }

    public void Update(AiProfesseur professeur)
    {
        //Debug.Log(Mathf.Sqrt(Vector3.Distance(professeur.transform.position, professeur.currentWaypoint.position)));
        if ((Mathf.Sqrt(Vector3.Distance(professeur.transform.position, professeur.currentWaypoint.position))) < 0.5f)
        {
            professeur.animator.SetBool("Scared", true);
        }

        if (professeur.targetAlarm == null)
        {
            professeur.animator.SetBool("Scared", false);
            professeur.SetSFX("Walk");

            professeur.professeurStateMachine.ChangeState(AiProfesseurStateId.Patrol);

        }
    }
}
