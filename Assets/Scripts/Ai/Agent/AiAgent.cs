using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class AiAgent : NetworkBehaviour
{
    float t = 0;
    public Transform head;
    public AiStateId initialState;
    public AiAgentConfig config;

    public bool alarme = false;
    public GameObject targetAlarme;
    public Transform targetCatch;

    public Transform iconAlarm;
    public Transform iconAlertness;
    public Transform iconChase;
    public SFXManager _SFXManager;
    public SFXManager voiceManager;

    private bool _stopCatch = false;

    [HideInInspector] public AiStateMachine stateMachine;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public AiSensor sensor;
    [HideInInspector] public AiTargetingSystem targeting;
    [HideInInspector] public AICharacter character;

    // Start is called before the first frame update
    void Start()
    {
        //component
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        character = GetComponent<AICharacter>();

        //vision
        sensor = GetComponent<AiSensor>();
        targeting = GetComponent<AiTargetingSystem>();

        //state machine
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiFindTargetState());
        stateMachine.RegisterState(new AiAlarmState());
        stateMachine.RegisterState(new AiStunState());
        stateMachine.RegisterState(new AiFallPaintState());
        stateMachine.RegisterState(new AiCaptureState());
        stateMachine.ChangeState(initialState);

        iconAlarm = this.transform.Find("AlarmeSymbol");
        iconAlarm.gameObject.SetActive(false);
        iconAlertness = this.transform.Find("AlertnessSymbol");
        iconAlertness.gameObject.SetActive(false);
        iconChase = this.transform.Find("ChaseSymbol");
        iconChase.gameObject.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();

    }

    public void SetSFX(string name)
    {
        _SFXManager.ChangeState(name);
    }

    public void SetVoice(string name)
    {
        voiceManager.ChangeState(name);
    }

    public IEnumerator ChangeToChaseMod()
    {
        animator.SetBool("Found", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Found", false);


        if (stateMachine.currentState != AiStateId.Stun)
        {

            navMeshAgent.enabled = true;
            //yield return new WaitForSeconds(1);
            if (stateMachine.currentState != AiStateId.Stun)
                stateMachine.ChangeState(AiStateId.ChasePlayer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //pt mettre quelque chose si deux joueurs rentre
        if (other.gameObject.layer == 8)
            t = 0;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && stateMachine.currentState != AiStateId.Capture && !_stopCatch)
        {
            PlayerCatched playerCatched = other.gameObject.GetComponent<PlayerCatched>();
            if (!playerCatched._catchedRun && playerCatched._canByCatch)
            {
                t += Time.deltaTime;
                if (t > config.tempsCatch)
                {
                    CatchPlayer(other.gameObject);
                }
            }
        }
    }

    public void CatchPlayer(GameObject player)
    {
        Debug.Log("AiAgent Catch Player");
        targetCatch = player.gameObject.transform;
        player.GetComponent<PlayerCatched>().PlayerCatchedStart(head, this);
        stateMachine.ChangeState(AiStateId.Capture);
        t = 0;
    }

    //public car player doit y acceder aussi?
    public void WinCapture()
    {
        animator.SetBool("Catch", false);
        Debug.Log("win");
        _stopCatch = true;
        StartCoroutine(ChangeStopCatch());
        stateMachine.ChangeState(AiStateId.FindTarget);
    }

    //public car player doit y acceder aussi?
    public void LooseCapture()
    {
        Debug.Log("loose");

        animator.SetBool("Catch", false);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("garde"))
            {
                hitCollider.GetComponent<AiAgent>().stateMachine.ChangeState(AiStateId.FallPaint);
                //hitCollider.GetComponent<AiAgent>().LooseCapture();
            }
        }

        _stopCatch = true;
        StartCoroutine(ChangeStopCatch());
        stateMachine.ChangeState(AiStateId.FallPaint);
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

    public IEnumerator ChangeStopCatch()
    {
        yield return new WaitForSeconds(4);
        _stopCatch = false;
    }
}
