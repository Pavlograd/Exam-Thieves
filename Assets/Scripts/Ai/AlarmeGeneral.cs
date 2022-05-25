using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmeGeneral : MonoBehaviour
{

    public AudioClip alarmeClip;
    private AudioSource audio;

    [HideInInspector] public AiAgent agent;
    [HideInInspector] public AiProfesseur aiProfesseur;

    //IA et player
    public GameObject[] agent_de_securite;
    public GameObject professeur;
    public GameObject target;
    //alarme pour boucle if
    public bool alarme;
    public bool alarmeAudio;
    public bool _alarmeFinal = false;

    public AiAgentConfig config;

    // Start is called before the first frame update
    void Start()
    {
        agent_de_securite = GameObject.FindGameObjectsWithTag("garde");
        audio = GetComponent<AudioSource>();
        alarmeAudio = false;
        audio.clip = alarmeClip;
        //audio.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (alarme)
        {

            alarmeAudio = true;
            Debug.Log("alarme turn on");
            aiProfesseur = professeur.GetComponent<AiProfesseur>();
            aiProfesseur.StartCoroutine(aiProfesseur.ChangeStateAlarme());
            transform.position = aiProfesseur.transform.position;
            foreach (GameObject agentSecu in agent_de_securite)
            {
                agent = agentSecu.GetComponent<AiAgent>();
                agent.alarme = true;
                agent.targetAlarme = target;
                Debug.Log(agent);
            }
            StartCoroutine(ArretAlarme());
        }
        if (alarmeAudio)
        {
            if (!audio.isPlaying)
            {
                audio.Play();
                //StartCoroutine(AudioManager());
            }

        }
        if (_alarmeFinal)
        {
            _alarmeFinal = false;
            alarmeAudio = true;
            transform.position = target.transform.position;

            foreach (GameObject agentSecu in agent_de_securite)
            {
                agent = agentSecu.GetComponent<AiAgent>();
                agent.alarme = true;
                agent.targetAlarme = target;
                Debug.Log(agent);
            }
        }
    }

    public void StopAlarme()
    {
        alarme = false;

        foreach (GameObject agentSecu in agent_de_securite)
        {
            agent = agentSecu.GetComponent<AiAgent>();
            agent.alarme = false;
            agent.targetAlarme = null;
            Debug.Log("arretalarme");

        }

        aiProfesseur = professeur.GetComponent<AiProfesseur>();
        aiProfesseur.targetAlarm = null;
        alarmeAudio = false;
        audio.Stop();
    }

    public void StopAlarmeCoroutine()
    {
        StopCoroutine(ArretAlarme());
    }

    IEnumerator ArretAlarme()
    {
        if (alarme)
        {
            alarme = false;
            Debug.Log("Stop alarme");
            yield return new WaitForSeconds(config.tempsAlarmeSecondes);
            alarmeAudio = false;
            StopAlarme();
        }
    }

    IEnumerator AudioManager()
    {
        yield return new WaitForSeconds(5);
        audio.Play();
    }
}
