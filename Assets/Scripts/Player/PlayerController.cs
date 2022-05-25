using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Mirror;
using StaticClassSettingsGame;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : NetworkBehaviour
{
    [Header("Player Mouvement option")]
    [SerializeField] private float speed;
    [SerializeField] private float speedRun;
    public SFXManager _SFXManager;
    public SFXManager _SFXManagerEffects;


    private float staminaRegenSpeed;
    private float staminaBurnSpeed;

    private float currentStamina = 1f;

    private bool waitStaminaReload = false;

    private float catchedCDCurrent = 0f;
    private float catchedCDPlayerCurrent = 0f;

    public bool pause = false;
    public bool canMove = true;

    public PlayerCatched _playerCatched;
    public PlayerSetup _playerSetup;
    private Vector2 move;
    private Vector2 moveLook;
    private PlayerControls _controls;
    public Player _player;

    public PlayerMotor _playerMotor;
    private bool sprintPressed = false;
    public bool isRun = false;
    private Animator _animator;
    public GameObject interactableObject;
    public PlayerCharacter _playerCharacter;

    public MinimapUpdate minimap;

    private bool shouldCrounch = false;
    // Start is called before the first frame update

    void Awake()
    {
        _controls = new PlayerControls();
        _controls.Player.Movement.performed += ctx => move = ctx.ReadValue<Vector2>();
        _controls.Player.Look.performed += ctx => moveLook = ctx.ReadValue<Vector2>();
        _controls.PlayerAction.Sprint.performed += ctx => sprintPressed = true;
        _controls.PlayerAction.Sprint.canceled += ctx => sprintPressed = false;
        _controls.PlayerAction.Pause.canceled += ctx => pause = !pause;
        //Je peux changer en performed en rajoutant une variable de check sur le cancel

    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    void Start()
    {
        _playerCatched = gameObject.GetComponent<PlayerCatched>();
        _playerMotor = gameObject.GetComponent<PlayerMotor>();
        _animator = gameObject.GetComponent<Animator>();
        _playerSetup = gameObject.GetComponent<PlayerSetup>();
        _player = gameObject.GetComponent<Player>();
        Cursor.lockState = CursorLockMode.Locked;
        SetupStamina();
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public float GetCurrentCatchedCD()
    {
        return catchedCDCurrent;
    }

    public float GetCurrentCatchedPlayerCD()
    {
        return catchedCDPlayerCurrent;
    }

    public bool GetwWaitStaminaReload()
    {
        return waitStaminaReload;
    }

    public void SetupStamina()
    {
        PlayerData playerData = gameObject.GetComponent<PlayerGFX>().currentPlayerData;
        staminaRegenSpeed = playerData.staminaRegenSpeed;
        staminaBurnSpeed = playerData.staminaBurnSpeed;
    }

    public void ResetCamera()
    {
        if (isLocalPlayer) _playerCatched.ResetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; }

        if (_player.isDeath)
        {
            ActionWhenBlock();
            return;
        }

        if (_playerCatched._catchedRun)
        {
            PowerAction();
            CatchedAction();
            return;
        }

        if (PauseMenu.isOn)
        {
            onPauseAction();
            return;
        }

        InteractAction();
        PingAction();

        if (!canMove)
        {
            ActionWhenBlock();
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;

        float xMov = move.x;
        float yMov = move.y;
        float yRot = moveLook.x;
        float xRot = moveLook.y;

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * yMov;

        Vector3 yRotation;
        float cameraRotationX;

        yRotation = new Vector3(0, yRot, 0) * (PlayerSettings.gamePadSensitivityX * 50);

        cameraRotationX = xRot * (PlayerSettings.gamePadSensitivityY * 50);

        SFX(moveHorizontal == Vector3.zero && moveVertical == Vector3.zero);
        PowerAction();
        Vector3 velocity = (moveHorizontal + moveVertical) * speed;
        SprintAction(moveHorizontal == Vector3.zero && moveVertical == Vector3.zero);
        if (yMov < 0f)
        {
            _animator.SetFloat("Speed", -0.5f);
            isRun = false;
        }
        else
        {
            if (isRun)
                velocity = (moveHorizontal + moveVertical) * speedRun;
            float speedAnimator = velocity == Vector3.zero ? 0f : 0.5f;
            speedAnimator = isRun && speedAnimator != 0f ? 1 : speedAnimator;
            _animator.SetFloat("Speed", speedAnimator);
        }
        StaminaGestion();
        CrounchAction();
        _playerMotor.Move(velocity, yRotation, cameraRotationX, isRun, shouldCrounch, false);
    }

    private void CrounchAction()
    {
        if (_controls.PlayerAction.Crounch.WasPressedThisFrame() && !PauseMenu.isOn)
        {
            shouldCrounch = true;
        }
        else
        {
            shouldCrounch = false;
        }
    }

    private void SFX(bool isNotMoving)
    {
        if (isNotMoving)
        {
            if (_SFXManager.GetState() != "Storage") ChangeSFXState("Idle");
        }
        else if (isRun)
        {
            ChangeSFXState("Run");
        }
        else if (shouldCrounch)
        {
            ChangeSFXState("Crounch");
        }
        else
        {
            ChangeSFXState("Walk");
        }
    }

    private void PowerAction()
    {
        if (_controls.PlayerAction.Power.WasPressedThisFrame())
        {
            Debug.Log("Use power (Check if client is ready)");
            CallPowerAction(this);
        }
    }



    [Command]
    void CallPowerAction(PlayerController pController)
    {
        SendPowerAction(pController);
        Debug.Log("Receive powerAction");
        pController._playerCharacter.TryUsePower();
    }

    [ClientRpc]
    void SendPowerAction(PlayerController pController)
    {
        Debug.Log("Receive powerAction");
        pController._playerCharacter.TryUsePower();
    }

    private void PingAction()
    {
        if (_controls.PlayerAction.Ping.WasPressedThisFrame())
        {
            Debug.Log("Use Ping (Check if client is ready)");
            CallPingAction(this);
        }
    }



    [Command]
    void CallPingAction(PlayerController pController)
    {
        SendPingAction(pController);
    }

    [ClientRpc]
    void SendPingAction(PlayerController pController)
    {
        Debug.Log("Receive powerAction");
        pController.minimap.CreatePing(pController);
        _SFXManagerEffects.ChangeState("Ping");
    }

    void ChangeSFXState(string state)
    {
        if (_SFXManager.GetState() != state)
        {
            _SFXManager.ChangeState(state);
        }
    }

    private void StaminaGestion()
    {
        if (isRun)
        {
            currentStamina -= staminaBurnSpeed * Time.deltaTime;
        }
        else
        {
            currentStamina += staminaRegenSpeed * Time.deltaTime;
        }
        if (currentStamina <= 0)
        {
            waitStaminaReload = true;
        }
        else if (currentStamina >= 1 && waitStaminaReload)
        {
            waitStaminaReload = false;
        }
        currentStamina = Mathf.Clamp(currentStamina, 0f, 1f);
    }

    private void SprintAction(bool isNotMoving)
    {
        if (!isNotMoving && sprintPressed && currentStamina > 0 && !waitStaminaReload)
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
    }

    private void InteractAction()
    {
        if (_controls.PlayerAction.Interaction.WasPressedThisFrame() && interactableObject != null)
        {
            Debug.Log("interact with: " + interactableObject.name);
            //interactableObject.SendMessage("Interacted", this);
            if (interactableObject.CompareTag("InterractOnlyServ"))
            {
                interactableObject.SendMessage("Interacted", this);
                Debug.Log("YES SIR");
            }
            else
            {
                CallInteraction(this);
            }
            //otherPlayerController.SendInteraction(this, interactableObject);
        }
    }

    [Command]
    void CallInteraction(PlayerController pController)
    {
        SendInteraction(pController);
    }

    [ClientRpc]
    void SendInteraction(PlayerController pController)
    {
        Debug.Log("Receive interaction");
        if (pController.interactableObject != null)
        {
            pController.interactableObject.SendMessage("Interacted", pController);
        }
    }

    private void onPauseAction()
    {
        if (Cursor.lockState != CursorLockMode.None)
            Cursor.lockState = CursorLockMode.None;
        ActionWhenBlock();
    }

    private void ActionWhenBlock()
    {
        if (_SFXManager.GetState() != "Idle") _SFXManager.ChangeState("Idle");
        _animator.SetFloat("Speed", 0f);
        currentStamina += staminaRegenSpeed * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, 1f);
        isRun = false;
        _playerMotor.Move(Vector3.zero, Vector3.zero, 0f, false, false, _playerCatched._catchedRun);
    }

    private void CatchedAction()
    {
        ActionWhenBlock();
        catchedCDCurrent += Time.deltaTime;
        catchedCDCurrent = Mathf.Clamp(catchedCDCurrent, 0.0f, GameManager.instance.secondToEscape);
        _playerMotor._vignette.smoothness.overrideState = true;
        _playerMotor._vignette.intensity.value = catchedCDCurrent / GameManager.instance.secondToEscape;
        _playerMotor._vignette.smoothness.value = _playerMotor._vignette.intensity.value;
        Gamepad.current.SetMotorSpeeds(catchedCDCurrent / GameManager.instance.secondToEscape < 0.5 ? 0.25f : 0.50f, catchedCDCurrent / GameManager.instance.secondToEscape);
        if (_controls.PlayerAction.Interaction.WasPressedThisFrame())
        {
            _playerCatched.PressA();
            catchedCDPlayerCurrent += 1;
            catchedCDPlayerCurrent = Mathf.Clamp(catchedCDPlayerCurrent, 0.0f,
                GameManager.instance.GetNbPressToEscapeDivide());
            if (catchedCDPlayerCurrent / GameManager.instance.GetNbPressToEscapeDivide() >= 1)
            {
                WinCatch();
            }
        }

        if (catchedCDCurrent / GameManager.instance.secondToEscape >= 1f)
        {
            LooseCatch();
        }
    }


    public void WinCatch()
    {
        Gamepad.current.SetMotorSpeeds(0, 0);
        Debug.Log("Player escape");
        _playerMotor._vignette.smoothness.overrideState = false;
        catchedCDPlayerCurrent = 0;
        catchedCDCurrent = 0;
        _playerCatched.PlayerCatchedEnd(false);
        CmdAddNumberCatch();
        return;
    }

    private void LooseCatch()
    {
        Gamepad.current.SetMotorSpeeds(0, 0);
        Debug.Log("Player attrapé");
        _playerMotor._vignette.smoothness.overrideState = false;
        gameObject.transform.position = _playerSetup.startPosition;
        catchedCDPlayerCurrent = 0;
        catchedCDCurrent = 0;
        CmdLooseLife();
        if (GameManager.instance.numberOfLife <= 0)
        {
            Debug.Log("Tu dois mourrir now");
            CmdDeathPlayer();
        }
        Debug.Log(GameManager.GetAllPlayer().Length + " " + GameManager.AllDeath());
        _playerCatched.PlayerCatchedEnd(true);
    }

    [Command]
    private void CmdDeathPlayer()
    {
        gameObject.GetComponent<Player>().isDeath = true;
    }


    [Command]
    private void CmdLooseLife()
    {
        GameManager.instance.numberOfLife--;
        GameManager.instance.numberCatch = 0;
    }

    [Command]
    private void CmdAddNumberCatch()
    {
        GameManager.instance.numberCatch++;
    }
}
