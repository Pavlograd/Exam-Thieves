using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public abstract class AICharacter : NetworkBehaviour
{
    public NavMeshAgent agent;
    [SyncVar]
    public int size = 0;

    // Can't use Start directly here
    public void CharacterStart()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    //Fonction de recherche
    public abstract void Seek(Vector3 location);

    //Fonction de fuite a appele
    public abstract void Flee(Vector3 location);

    public abstract void Wander();
}