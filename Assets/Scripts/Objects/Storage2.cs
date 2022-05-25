using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage2 : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] BoxCollider boxTrigger;
    [SerializeField] Interactable _interactable;
    public GameObject goToSetActive;

    public void Interacted(PlayerController _playerController)
    {
        if (goToSetActive != null) goToSetActive.SetActive(true);

        SFXManager sfxInteractions = GameObject.Find("SFX_INTERACTIONS").GetComponent<SFXManager>();

        sfxInteractions.transform.position = transform.position;
        sfxInteractions.ChangeState("Storage");

        _playerController.interactableObject = null;
        _interactable.UnHighlight();
        DestroyImmediate(_interactable);
        DestroyImmediate(boxTrigger);
        _animator.SetBool("Opened", !_animator.GetBool("Opened"));
        Invoke("DestroyItself", 1.0f);
    }

    void DestroyItself()
    {
        DestroyImmediate(_animator);
        DestroyImmediate(this);
    }
}
