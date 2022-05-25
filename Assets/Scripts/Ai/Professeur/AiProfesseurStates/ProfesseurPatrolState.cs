using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfesseurPatrolState : AiProfesseurState
{
    int count;
    bool hasSeenPlayer = false;

    public void Enter(AiProfesseur professeur)
    {
        professeur.currentWaypoint = professeur.waypoints.GetNextWaypoint(professeur.currentWaypoint);
        professeur.Seek(professeur.currentWaypoint.position);
        //Debug.Log("0");
        professeur.navMeshAgentProf.speed = 1.5f;
        professeur.SetSFX("Walk");
        //professeur.iconfleche.gameObject.SetActive(true);
        hasSeenPlayer = false;
    }

    public void Exit(AiProfesseur professeur)
    {
        professeur.iconVision.gameObject.SetActive(false);
        professeur.iconVisionNo.gameObject.SetActive(false);
        //professeur.iconfleche.gameObject.SetActive(false);
        //professeur.ResetAllTriggers();
    }

    public AiProfesseurStateId GetId()
    {
        return AiProfesseurStateId.Patrol;
    }

    void DetectPlayer(AiProfesseur professeur)
    {
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(professeur.transform.position, 1.5f, hitColliders);

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].gameObject.gameObject.layer == 8)
            {
                Debug.Log("Player detected ! Look at !!");
                professeur.SetVoice("Ah");
                professeur.transform.LookAt(hitColliders[i].transform);
                professeur.targetAlarm = hitColliders[i].gameObject;
                professeur.iconAlertness.gameObject.SetActive(false);
                professeur.StartCoroutine(professeur.ChangeToWantsAlarmMod());
                hasSeenPlayer = true;
            }
        }
    }

    public void Update(AiProfesseur professeur)
    {
        if (!professeur.iconAlertness.gameObject.activeSelf && !professeur.iconChase.gameObject.activeSelf)
        {
            professeur.iconNormal.gameObject.SetActive(true);
        }
        else
        {
            professeur.iconNormal.gameObject.SetActive(false);
        }
        if (hasSeenPlayer) return;

        DetectPlayer(professeur);

        if (hasSeenPlayer) return;

        int index_waypoint = professeur.indexWaypoints + 1;

        if (index_waypoint > professeur.waypoints.transform.childCount - 1)
        {
            index_waypoint = 0;
        }

        if (professeur.targeting.HasTarget)
        {
            if (Vector3.Distance(professeur.targeting.Target.transform.position, professeur.transform.position) > (DifficultySettings.datas.FOVGuards * 0.60))
            {
                professeur.iconAlertness.gameObject.SetActive(true);
                professeur.navMeshAgentProf.speed = 0.4f;
                professeur.Seek(professeur.targeting.Target.transform.position);
            }
            else
            {
                professeur.targetAlarm = professeur.targeting.Target;
                professeur.iconAlertness.gameObject.SetActive(false);
                professeur.transform.LookAt(professeur.targeting.Target.transform);
                professeur.StartCoroutine(professeur.ChangeToWantsAlarmMod());
                hasSeenPlayer = true;
            }
        }
        else if (professeur.navMeshAgentProf.enabled)
        {
            professeur.iconAlertness.gameObject.SetActive(false);
            professeur.Seek(professeur.currentWaypoint.position);
        }

        //Debug.Log(Mathf.Sqrt(Vector3.Distance(professeur.navMeshAgentProf.transform.position, professeur.currentWaypoint.position)));
        /* if (Mathf.Sqrt(Vector3.Distance(professeur.navMeshAgentProf.transform.position, professeur.currentWaypoint.position)) < 0.5f)
         {
             //professeur.StartCoroutine(professeur.WaitWaypoints());
             professeur.currentWaypoint = professeur.waypoints.GetNextWaypoint(professeur.currentWaypoint);
             professeur.Seek(professeur.currentWaypoint.position);
             if (professeur.indexWaypoints % 2 == 1)
             {
                 professeur.iconVisionNo.gameObject.SetActive(true);
                 professeur.iconVision.gameObject.SetActive(false);
                 professeur.sensor.enabled = false;
                 professeur.targeting.enabled = false;
                 professeur.navMeshAgentProf.speed = 1f;

             }
             if (professeur.indexWaypoints % 2 == 0)
             {
                 professeur.sensor.enabled = true;
                 professeur.targeting.enabled = true;
                 professeur.navMeshAgentProf.speed = 1.5f;
                 professeur.iconVisionNo.gameObject.SetActive(false);
                 professeur.iconVision.gameObject.SetActive(true);
             }

         }
         else if (professeur.navMeshAgentProf.enabled)
         {
             professeur.Seek(professeur.currentWaypoint.position);
         }*/

        if (Mathf.Sqrt(Vector3.Distance(professeur.navMeshAgentProf.transform.position, professeur.currentWaypoint.position)) < 0.5f)
        {
            
            professeur.currentWaypoint = professeur.waypoints.GetNextWaypoint(professeur.currentWaypoint);
            professeur.Seek(professeur.currentWaypoint.position);
            professeur.StartCoroutine(professeur.WaitWaypoints());
        }
        else if (professeur.navMeshAgentProf.enabled)
        {
            professeur.Seek(professeur.currentWaypoint.position);
        }
     }
    }


