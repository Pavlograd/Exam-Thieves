using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    List<GameObject> players = new List<GameObject>();
    public bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player inside");
            players.Add(other.gameObject);
            playerInside = true;
        }
    }

    // Handle exiting the trigger
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            players.Remove(other.gameObject);

            if (players.Count == 0)
            {
                Debug.Log("Player Outside");
                playerInside = false;
            }
        }
    }
}
