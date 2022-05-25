using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : MonoBehaviour
{
    [SerializeField] Animator _animator;

    void Start()
    {
        Invoke("ReScale", 1.5f);
    }

    void ReScale()
    {
        transform.localScale = Vector3.one;
    }

    public void Interacted(PlayerController pController)
    {
        Debug.Log("Rpc annimation locker");
        bool opened = _animator.GetBool("Opened");

        _animator.SetBool("Opened", !opened);
    }
}
