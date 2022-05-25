using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleDeath))]
    public bool isDeath = false;

    [SerializeField]
    private Behaviour[] compenentsToDisableSpectator;

    [SerializeField] private Camera _camera;
    public void HandleDeath(bool oldValue, bool newValue) => Death();
    // Start is called before the first frame update


    private void Death()
    {
        if (isDeath)
        {
            if (GameManager.AllDeath())
            {
                Debug.Log("Lancer la fin de jeu");
                Defeat();
            }
            else
            {
                Debug.Log("Tp du joueur sur l'autre points de vu");
                ModeSpectator();
            }
        }
    }

    [Command]
    public void Defeat()
    {
        GameManager.instance.endGame = true;
        GameManager.instance.Defeat();
        GameManager.instance.StopTimer();
    }

    private void ModeSpectator()
    {
        gameObject.GetComponentInChildren<SFXManager>().StopSong();
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(false);
        }
        TpPlayerSpectator();
        DisableComponents();
    }

    private void DisableComponents()
    {
        for (int i = 0; i != compenentsToDisableSpectator.Length; i++)
        {
            compenentsToDisableSpectator[i].enabled = false;
        }
        gameObject.GetComponent<CharacterController>().enabled = false;
    }

    private void TpPlayerSpectator()
    {
        foreach (Player player in GameManager.GetAllPlayer())
        {
            if (!player.isDeath)
            {
                player._camera.enabled = true;
                gameObject.GetComponent<PlayerSetup>().ActiveLayer(player.gameObject.GetComponent<PlayerGFX>().playerGFXInstance);
                break;
            }
        }
    }
}
