using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class AiProfesseur : NetworkBehaviour
{
    //waypoints
    [SerializeField] public Waypoints waypoints;
    [HideInInspector] public Transform currentWaypoint;
    [HideInInspector] public int indexWaypoints;

    [HideInInspector] public AiTargetingSystem targeting;
    [HideInInspector] public AiSensor sensor;

    [HideInInspector] public NavMeshAgent navMeshAgentProf;
    [HideInInspector] public Animator animator;

    public Transform iconAlarm;
    public Transform iconAlertness;
    public Transform iconChase;
    public Transform iconNormal;
    public Transform iconVision;
    public Transform iconVisionNo;



    //state machine
    public AiProfesseurStateMachine professeurStateMachine;
    public AiProfesseurStateId initialState;
    //config
    public AiAgentConfig config;
    [SerializeField] SFXManager _SFXManager;
    [SerializeField] SFXManager voiceManager;
    //alarme

    [HideInInspector] public GameObject targetAlarm;
    [SyncVar]
    public int idAlarm = 0;

    // Start is called before the first frame update
    public void Start()
    {
        //vision
        sensor = GetComponent<AiSensor>();
        targeting = GetComponent<AiTargetingSystem>();

        iconAlarm = this.transform.Find("AlarmeSymbol");
        iconAlarm.gameObject.SetActive(false);
        iconAlertness = this.transform.Find("AlertnessSymbol");
        iconAlertness.gameObject.SetActive(false);
        iconChase = this.transform.Find("ChaseSymbol");
        iconChase.gameObject.SetActive(false);
        iconNormal = this.transform.Find("NormalCercle");
        iconNormal.gameObject.SetActive(false);
        iconVision = this.transform.Find("VisionSymbol");
        iconVision.gameObject.SetActive(false);
        iconVisionNo = this.transform.Find("NoVisionSymbol");
        iconVisionNo.gameObject.SetActive(false);

        //component
        navMeshAgentProf = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        //Set le prochain waypoint
        /*currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        Seek(currentWaypoint.position);*/

        //state machine
        professeurStateMachine = new AiProfesseurStateMachine(this);
        professeurStateMachine.RegisterState(new ProfesseurPatrolState());
        professeurStateMachine.RegisterState(new ProfesseurWantsToAlarmState());
        professeurStateMachine.RegisterState(new ProfesseurAlarmState());
        professeurStateMachine.RegisterState(new ProfesseurFallState());
        professeurStateMachine.RegisterState(new ProfesseurStunState());

        professeurStateMachine.ChangeState(initialState);
    }

    public void Init(Waypoints _waypoints)
    {
        waypoints = _waypoints;

        //waypoints variables
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints != null)
        {
            professeurStateMachine.Update();
            indexWaypoints = waypoints.Index_Waypoint(currentWaypoint);
        }
    }

    public void Seek(Vector3 location)
    {
        navMeshAgentProf.SetDestination(location);
    }

    public IEnumerator ChangeToWantsAlarmMod()
    {
        Debug.Log("Found");
        //navMeshAgentProf.isStopped = true;
        iconNormal.gameObject.SetActive(false);
        navMeshAgentProf.enabled = false;
        animator.SetBool("Found", true);

        yield return new WaitForSeconds(1f);
        animator.SetBool("Found", false);
        professeurStateMachine.ChangeState(AiProfesseurStateId.WantsToAlarm);

        /*if (professeurStateMachine.currentState != AiProfesseurStateId.Stun)
        {

            SetTrigger("Run");
            //yield return new WaitForSeconds(1);
            if (professeurStateMachine.currentState != AiProfesseurStateId.Stun)
                professeurStateMachine.ChangeState(AiProfesseurStateId.WantsToAlarm);
        }*/
    }
    public IEnumerator ChangeStateAlarme()
    {
        animator.SetBool("Intercom", true);
        SetSFX("Intercom");
        //navMeshAgentProf.isStopped = true;
        navMeshAgentProf.enabled = false;

        yield return new WaitForSeconds(6f);
        animator.SetBool("Intercom", false);
        navMeshAgentProf.enabled = true;
        professeurStateMachine.ChangeState(AiProfesseurStateId.Alarm);
    }
    public void ResetAllTriggers()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }

    public void SetSFX(string name)
    {
        _SFXManager.ChangeState(name);
    }

    public void SetVoice(string name)
    {
        voiceManager.ChangeState(name);
    }

    public void SetTrigger(string name)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger && param.name != name)
            {
                animator.ResetTrigger(param.name);
            }
            else if (param.type == AnimatorControllerParameterType.Trigger && param.name == name)
            {
                animator.SetTrigger(param.name);
            }
        }
    }

    public IEnumerator WaitWaypoints()
    {
        navMeshAgentProf.speed = 0f;
        yield return new WaitForSeconds(1);
        if (indexWaypoints % 2 == 1)
        {
            iconVisionNo.gameObject.SetActive(true);
            iconVision.gameObject.SetActive(false);
            sensor.enabled = false;
            targeting.enabled = false;
            navMeshAgentProf.speed = 1f;

        }
        if (indexWaypoints % 2 == 0)
        {
            sensor.enabled = true;
            targeting.enabled = true;
            navMeshAgentProf.speed = 1.5f;
            iconVisionNo.gameObject.SetActive(false);
            iconVision.gameObject.SetActive(true);
        }

    }
}

