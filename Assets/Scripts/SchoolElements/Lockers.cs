using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockers : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] Interactable _interactable;
    [SerializeField] GameObject player;
    [SerializeField] Color openColor;
    [SerializeField] Color closeColor;
    [SerializeField] Locker[] _lockers;
    [SerializeField] BoxCollider[] _colliders;
    [SerializeField] Locker _locker;

    void Awake()
    {
        int index = Random.Range(0, (int)DifficultySettings.datas.percentageLockers);

        //Debug.Log("Start locker");
        index = 10;
        if (index <= 3)
        {
            _locker = _lockers[index];
            _locker.ActivateItself(openColor);
            _interactable.meshRenderer = _locker._MeshDoor;
            _animator = _locker._animator;
        }

        for (int i = 0; i < _lockers.Length; i++)
        {
            if (i != index)
            {
                Destroy(_colliders[i]);
                _lockers[i].DestroyItself(closeColor);
            }
        }

        if (index > 3)
        {
            Destroy(_interactable);
            Destroy(this);
        }
    }

    public void Interacted(PlayerController _playerController)
    {
        if (player == null)
        {
            _playerController._SFXManagerEffects.ChangeState("Locker");

            Debug.Log("Null");

            _animator.Play("CloseDoor", -1, 0.0f);

            // HERE PLAYER ENTERS LOCKER
            player = _playerController.gameObject;
            player.transform.position = _locker.insidePosition.position;
            _playerController.canMove = false;
            _playerController.ResetCamera();
            _playerController.transform.rotation = _locker.rotation * Quaternion.Euler(0.0f, 0.0f, 0.0f);
            if (_playerController.isLocalPlayer) _playerController._playerCatched._lockers = this;
            player.gameObject.layer = 0;
        }
        else if (player == _playerController.gameObject)
        {
            _playerController._SFXManager.ChangeState("Locker");

            Debug.Log("Not Null");
            ToggleDoor();
            Invoke("ToggleDoor", 1.0f);
            // HERE PLAYERS EXITS LOCKER
            player.transform.position = _locker.outsidePosition.position;
            player.gameObject.layer = 8;
            _playerController.canMove = true;
            if (_playerController.isLocalPlayer) _playerController._playerCatched._lockers = null;
            player = null;
        }
    }

    void Update()
    {
        // Replace player if they have been pushed
        if (player != null)
        {
            _interactable.promptMessage = "Press       To Exit";
            player.transform.position = _locker.insidePosition.position;
        }
        else
        {
            _interactable.promptMessage = "Press       To Enter";
        }
    }

    /*void CallToggleDoor()
    {
        Debug.Log("Command locker");
        ToggleDoor();
    }*/

    void ToggleDoor()
    {
        Debug.Log("Rpc annimation locker");
        bool opened = _animator.GetBool("Opened");

        _animator.SetBool("Opened", !opened);
    }

    void GuardInspection(GameObject guard)
    {
        Debug.Log("Guard inspect");
        ToggleDoor();

        if (player != null)
        {
            // Move guard in front of locker

            AiAgent agent = guard.GetComponent<AiAgent>();

            StartCoroutine(CatchPlayer(guard));

            agent.navMeshAgent.SetDestination(_locker.outsidePosition.position);
        }
        else
        {
            Invoke("ToggleDoor", 1.0f);
        }
    }

    IEnumerator CatchPlayer(GameObject guard)
    {
        yield return new WaitForSeconds(1.0f);

        if (player == null) yield break;

        // CAPTURE PLAYER HERE
        //player.transform.position = _locker.outsidePosition.position;

        player.gameObject.layer = 8;
        player.GetComponent<PlayerController>().canMove = true;

        AiAgent agent = guard.GetComponent<AiAgent>();

        agent.CatchPlayer(player);
    }

    public void MovePlayerOutOfLocker()
    {
        ToggleDoor();
        player.transform.position = _locker.outsidePosition.position;
        player = null;
    }
}
