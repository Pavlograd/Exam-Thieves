using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarme : MonoBehaviour
{
    public AiProfesseur aiProfesseur;
    public AlarmeGeneral alarmeGeneral;
    public GameObject AlarmeManager;
    public GameObject professeur;

    // Start is called before the first frame update
    void Start()
    {
        // Can't add this in scene Game
        //aiProfesseur = professeur.gameObject.GetComponent<AiProfesseur>();
        //alarmeGeneral = AlarmeManager.GetComponent<AlarmeGeneral>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggeralarme");
        if (other.gameObject == professeur && aiProfesseur.targetAlarm != null)
        {
            Debug.Log("targetalarme non null");
            alarmeGeneral.professeur = professeur;
            alarmeGeneral.target = this.gameObject;
            alarmeGeneral.alarme = true;
        }
    }
}
