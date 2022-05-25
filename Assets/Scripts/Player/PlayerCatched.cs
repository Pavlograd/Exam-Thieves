using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCatched : NetworkBehaviour
{
    public PlayerController _playerController;
    public bool _catchedRun = false;
    private PlayerUI _playerUI;
    [SerializeField] private CameraShake _cameraShake;
    private AiAgent _aiAgent = null;
    public bool _canByCatch = true;
    [SerializeField] private float timeWaitAfterCatch;
    public Lockers _lockers = null;
    private Quaternion rotation;

    void Start()
    {
        _playerController = gameObject.GetComponent<PlayerController>();
        _playerUI = gameObject.GetComponent<PlayerSetup>().playerUIInstance.GetComponent<PlayerUI>();
    }

    public void PlayerCatchedStart(Transform headGardTransform, AiAgent aiAgent)
    {
        Debug.Log(headGardTransform.position);
        Debug.Log("Start catched Player");
        gameObject.GetComponent<Animator>().SetBool("catch", true);
        Debug.Log("Can't be Catch to the moment");
        //rotation = gameObject.transform.rotation;
        //gameObject.transform.LookAt(headGardTransform);
        _catchedRun = true;
        _canByCatch = false;
        if (_cameraShake.enabled)
            _cameraShake.StartShake(headGardTransform);
        _aiAgent = aiAgent;
    }

    public void PressA()
    {
        _playerUI.CatchedPlayerPressA();
    }

    public void ResetCamera()
    {
        _cameraShake.SetCameraFront();
    }

    public void PlayerCatchedEnd(bool catched)
    {
        Debug.Log("End catched Player");
        Gamepad.current.SetMotorSpeeds(0, 0);
        _catchedRun = false;
        gameObject.GetComponent<Animator>().SetBool("catch", false);
        //gameObject.transform.rotation = rotation;
        Debug.Log("Agent : " + _aiAgent);

        if (_cameraShake.enabled)
            _cameraShake.StopShake();
        if (catched) //si il c'est fait catch
        {
            CmdLooseCatch(_aiAgent);
            _playerUI.PrintRespawn();
        }
        else
        {
            CmdWinCatch(_aiAgent);
        }

        if (_lockers != null)
        {
            _lockers.MovePlayerOutOfLocker();
            _playerController.canMove = true;
        }

        _lockers = null;
        _aiAgent = null;
        StartCoroutine(Wait());
    }

    [Command]
    void CmdWinCatch(AiAgent agent)
    {
        if (agent == null) return;
        agent.LooseCapture();
        WinCatch(agent);
    }

    [ClientRpc]
    void WinCatch(AiAgent agent)
    {
        agent.LooseCapture();
    }

    [Command]
    void CmdLooseCatch(AiAgent agent)
    {
        if (agent == null) return;
        agent.WinCapture();
        LooseCatch(agent);
    }

    [ClientRpc]
    void LooseCatch(AiAgent agent)
    {
        if (agent != null) agent.WinCapture();
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(timeWaitAfterCatch);
        _canByCatch = true;
        Debug.Log("Can be Catch");
    }
}
