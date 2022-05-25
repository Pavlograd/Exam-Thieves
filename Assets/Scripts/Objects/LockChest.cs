using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LockChest : NetworkBehaviour
{
    [SerializeField] Animator _animator;

    [Command(requiresAuthority = false)]
    public void Interacted(PlayerController _playerController)
    {
        if (GameManager.instance.numberOfCode == GameManager.instance.numberMaxCode)
        {
            Debug.Log("Open LockChest");
            GameManager.instance.StopTimer();
            GameManager.instance.endGame = true;
            GameManager.instance.Victory();
            _animator.SetBool("Opened", true);

            Destroy(this); // Just destroy to know if it has been activated
        }
        else
        {
            Debug.Log("Try Open LockChest");
        }
    }
}
