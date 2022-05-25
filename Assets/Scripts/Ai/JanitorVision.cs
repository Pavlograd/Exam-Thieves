using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JanitorVision : MonoBehaviour
{
    public AiSensor sensor;
    public AiTargetingSystem targeting;
    public JanitorAI janitor;
    public Transform iconText;
    public NavMeshAgent navMeshAgent;
    public SFXManager _SFXManager;
    public Transform arretIcon;

    // Start is called before the first frame update
    void Start()
    {
        _SFXManager.ChangeState("Loop");
        iconText = this.transform.Find("TextSymbol");
        iconText.gameObject.SetActive(false);

        sensor = this.GetComponent<AiSensor>();
        targeting = this.GetComponent<AiTargetingSystem>();
        janitor = this.GetComponent<JanitorAI>();
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        arretIcon = this.transform.Find("ArretSymbol");
        arretIcon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        janitor.Wander();

        if (targeting.HasTarget)
        {
            if (_SFXManager.GetState() != "Out") _SFXManager.ChangeState("Out");
            arretIcon.gameObject.SetActive(true);
            iconText.gameObject.SetActive(true);
            //navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
        }
        else
        {
            if (_SFXManager.GetState() != "Loop") _SFXManager.ChangeState("Loop");
            //navMeshAgent.isStopped = false;
            iconText.gameObject.SetActive(false);
            arretIcon.gameObject.SetActive(false);
        }
    }
}
