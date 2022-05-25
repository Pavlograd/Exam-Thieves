using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfesseurWantsToAlarmState : AiProfesseurState
{
    public void Enter(AiProfesseur professeur)
    {
        //Debug.Log("1");
        professeur.navMeshAgentProf.enabled = true;
        professeur.navMeshAgentProf.speed = 2.5f;
        professeur.iconChase.gameObject.SetActive(true);
        professeur.iconNormal.gameObject.SetActive(false);
        professeur.Seek(professeur.waypoints.intercom.transform.position);
    }

    public void Exit(AiProfesseur professeur)
    {
        
        professeur.iconChase.gameObject.SetActive(false);
    }

    public AiProfesseurStateId GetId()
    {
        return AiProfesseurStateId.WantsToAlarm;
    }

    public void Update(AiProfesseur professeur)
    {
        
    }
}
