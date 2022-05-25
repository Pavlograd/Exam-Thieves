using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStartCatch : MonoBehaviour
{
    [SerializeField] private Transform headTransform;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCatched>().PlayerCatchedStart(headTransform, null);
        }
    }
}
