using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractableObject : MonoBehaviour
{
    [SerializeField] PlayerController pController;

    public PlayerController GetPlayerController()
    {
        return pController;
    }
}
