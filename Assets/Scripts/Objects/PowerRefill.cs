using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerRefill : MonoBehaviour
{
    private CharacterType _characterType;
    [SerializeField] GameObject[] refills;
    [SerializeField] Interactable _interactable;

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length == 0) return;

        CharacterType type1 = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerCharacter>().GetCharacterType();
        CharacterType type2 = GameObject.FindGameObjectsWithTag("Player")[1].GetComponent<PlayerCharacter>().GetCharacterType(); // SET TO 1 LATER

        _characterType = Random.Range(0, 2) == 1 ? type1 : type2;

        //transform.position = new Vector3(transform.position.x, 0.3f, transform.position.y);

        GameObject go = Instantiate(refills[(int)_characterType], transform.localPosition, Quaternion.identity, transform);

        // Will only highlight if Player is the same type as the refill
        if ((_characterType == type1 && GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerSetup>().isLocalPlayer) ||
        (_characterType == type2 && GameObject.FindGameObjectsWithTag("Player")[1].GetComponent<PlayerSetup>().isLocalPlayer)) // SET TO 1 LATER
        {
            _interactable.meshRenderer = go.transform.GetChild(0).GetComponent<MeshRenderer>();
            _interactable.Init();
        }
    }

    public void Interacted(PlayerController _playerController)
    {
        if (_playerController._playerCharacter.Refill(_characterType))
        {
            _playerController._SFXManagerEffects.ChangeState("Refill");
            Destroy(this.gameObject);
        }
        else
        {
            _playerController._SFXManagerEffects.ChangeState("Error");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerInteractableObject"))
        {
            PlayerController pController = other.gameObject.GetComponent<PlayerInteractableObject>().GetPlayerController();

            // Hide Prompt if already has power
            if (pController.gameObject.GetComponent<PlayerCharacter>().GetCharacterType() == _characterType && pController.gameObject.GetComponent<PlayerCharacter>().GetHasPower())
            {
                Debug.Log("Hide prompt");
                _interactable.promptMessage = "Already have power";
                _interactable.posPromptImage = Vector3.zero;
            }
            else
            {
                _interactable.promptMessage = "Press       To Collect";
                _interactable.posPromptImage = new Vector3(-55f, 0f, 0f);
            }
        }
    }
}
