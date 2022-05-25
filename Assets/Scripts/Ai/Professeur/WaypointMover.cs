using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointMover : MonoBehaviour
{
    public NavMeshAgent agent;

    //Reference au system de waypoint que cet objet va utiliser
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moveSpeed = 5f;
    
    

    //Le waypoint que le prof va vers
    private Transform currentWaypoint;

    // Start is called before the first frame update
    void Start()
    {
       
       agent = this.GetComponent<NavMeshAgent>();

        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;

        //Set le prochain waypoint
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        Seek(currentWaypoint.position);
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position =Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime); 


        if (Vector3.Distance(agent.transform.position, currentWaypoint.position) < 0.5f)
        {
            
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            Seek(currentWaypoint.position);
            //transform.LookAt(currentWaypoint);
        }
    }


    public  void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    public void MoveWaypoint()
    {
        if (Vector3.Distance(agent.transform.position, currentWaypoint.position) < 0.5f)
        {

            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            Seek(currentWaypoint.position);
            //transform.LookAt(currentWaypoint);
        }
    }
}
