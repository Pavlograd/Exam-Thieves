using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AiAgentConfig : ScriptableObject
{
    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    public float maxSightDistance = 5.0f;

    //Vitesse
    public float vitesseMarche = 2.5f;
    public float vitesseCourse = 3.5f;

    //Sensor
    public float distance = 10f;
    public float angle = 30f;
    public float height = 2f;
    public int scanFrequency = 30;

    //agent
    public float tempsCatch = 1f;

    //alarme
    public float tempsAlarmeSecondes = 10f;

    //pouvoir
    public float tempsStun = 2.0f;
    public float tempsFall = 3f; //3.5 est le temps max pour l'animation
}
