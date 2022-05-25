using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Code : NetworkBehaviour
{

    [Command(requiresAuthority = false)]
    public void Interacted(PlayerController _playerController)
    {
        Debug.Log("Grab the code");
        GameManager.instance.numberOfCode++;
        _playerController._SFXManagerEffects.ChangeState("Paper");
        Destroy(gameObject); // Just destroy to know if it has been activated
    }
}
