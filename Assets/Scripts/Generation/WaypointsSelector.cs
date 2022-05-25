using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointsSelector : MonoBehaviour
{
    [SerializeField] GameObject waypoints1;
    [SerializeField] GameObject waypoints2;

    void Awake()
    {
        if (waypoints2 == null)
        {
            DestroyImmediate(this);
            return;
        }
        if (Random.Range(0, 2) == 1)
        {
            DestroyImmediate(waypoints1);
        }
        else
        {
            DestroyImmediate(waypoints2);
        }
        Destroy(this);
    }
}
