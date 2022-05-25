using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class Waypoints : MonoBehaviour
{
    //video : https://www.youtube.com/watch?v=EwHiMQ3jdHw
    [Range(0f, 2f)]
    [SerializeField] private float waypointSize = 1f;
    public GameObject intercom;
    

    public void Init(int size)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.GetChild(i).position, out hit, size, NavMesh.AllAreas) && (int)hit.position.y == 0)
            {
                transform.GetChild(i).position = hit.position;
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(t.position, waypointSize);
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position); //la position dans la hierarchie determine l<ordre
        }
        Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
    }

    public Transform GetNextWaypoint(Transform currentWaypoint)
    {
        if (currentWaypoint == null)
        {
            return transform.GetChild(0);
        }
        if (currentWaypoint.GetSiblingIndex() < transform.childCount - 1)
        {
            return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
        }
        else
        {
            return transform.GetChild(0);
        }
    }

    public int Index_Waypoint(Transform currentWaypoint)
    {
        int index = currentWaypoint.GetSiblingIndex();
        return index;
    }
}
