using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerGFX : MonoBehaviour
{
    [SerializeField] private PlayerData[] PlayerDataList;
    [SerializeField] private Transform playerGFXHolder;
    [SerializeField] private PlayerCharacter _playerCharacter;

    [HideInInspector] public PlayerData currentPlayerData;

    public GameObject playerGFXInstance = null;

    private Animator _animator;

    private NetworkAnimator _networkAnimator;

    public void runPlayerGFX(string type, int skinColor, int hairColor)
    {
        Debug.Log(type);
        _animator = gameObject.GetComponent<Animator>();
        _networkAnimator = gameObject.GetComponent<NetworkAnimator>();
        foreach (PlayerData playerData in PlayerDataList)
        {
            if (playerData.name == type)
            {
                currentPlayerData = playerData;
                break;
            }
        }

        if (currentPlayerData == null)
            currentPlayerData = PlayerDataList[2];
        setPlayerGFX(skinColor, hairColor);
    }

    private void setPlayerGFX(int skinColor, int hairColor)
    {
        playerGFXInstance = Instantiate(currentPlayerData.skinPlayer, playerGFXHolder.position, playerGFXHolder.rotation);
        playerGFXInstance.name = currentPlayerData.nameObjAnimation;
        playerGFXInstance.transform.SetParent(playerGFXHolder);
        _animator.runtimeAnimatorController = currentPlayerData._animator;
        _networkAnimator.animator.Rebind();
        _animator.Rebind();
        _playerCharacter.SetCharacterType(currentPlayerData.type);
        gameObject.GetComponent<PlayerSetup>().ActiveLayer(playerGFXInstance);
        playerGFXInstance.GetComponent<PlayerCusto>().ChangeColor(skinColor, hairColor);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
