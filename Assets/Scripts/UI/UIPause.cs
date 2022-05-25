using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using StaticClassSettingsGame;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    private enum State
    {
        ResumeState,
        PauseionState,
        SettingState,
        LeaveGameState
    }

    [SerializeField] private GameObject PauseMenuGO;
    [SerializeField] private GameObject LeaveMenuGO;
    [SerializeField] private GameObject SettingsMenuGO;
    [SerializeField] private GameObject AudioGo;
    [SerializeField] private GameObject ControleGo;
    [SerializeField] private TMP_Text SettingTitle;
    [Header("Pause Menu")]
    [SerializeField] private Button RemuseButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button LeaveButton;
    [Header("Music Option")]
    [SerializeField] private Slider masterMusicSlider;
    [SerializeField] private Slider musicMusicSlider;
    [SerializeField] private Slider fxMusicSlider;
    [Header("Control Option")]
    [SerializeField] private Slider horizontalSlider;
    [SerializeField] private Slider verticalSlider;
    [SerializeField] private Toggle inverseAxisToggle;

    [Header("MatchMaker")]
    [SerializeField] private Matchmaker _matchmaker;

    [Header("Audio")]
    [SerializeField] private SFXManager _SFXManager;
    [Header("PlayerUI")]
    [SerializeField] private PlayerUI _playerUI;

    private void Start()
    {
        horizontalSlider.value = PlayerSettings.gamePadSensitivityX;
        verticalSlider.value = PlayerSettings.gamePadSensitivityY;

        inverseAxisToggle.isOn = PlayerSettings.inverseAxis;

        masterMusicSlider.value = PlayerSettings.masterMusicLevel;
        musicMusicSlider.value = PlayerSettings.musicMusicLevel;
        fxMusicSlider.value = PlayerSettings.fxMusicLevel;
    }

    private State _state = State.ResumeState;
    private PlayerControls _controls;

    public void ToggleHorizontalSlider() => PlayerSettings.gamePadSensitivityX = horizontalSlider.value;
    public void ToggleVerticalSlider() => PlayerSettings.gamePadSensitivityY = verticalSlider.value;
    public void ToggleInverseAxisToggle() => PlayerSettings.inverseAxis = inverseAxisToggle.isOn;

    public void ToggleMasterSlider()
    {
        PlayerSettings.masterMusicLevel = masterMusicSlider.value;
        GameObject.Find("Music").GetComponent<MusicManager>().ChangeVolume();
        _SFXManager.ChangeState("Select");
    }

    public void ToggleMusicSlider()
    {
        PlayerSettings.musicMusicLevel = musicMusicSlider.value;
        GameObject.Find("Music").GetComponent<MusicManager>().ChangeVolume();
        _SFXManager.ChangeState("Select");
    }
    public void ToggleFxSlider()
    {
        PlayerSettings.fxMusicLevel = fxMusicSlider.value;
        _SFXManager.ChangeState("Select");
    }

    public void Activate()
    {
        Debug.Log("Activate");
        gameObject.SetActive(true);
        PauseMenuGO.SetActive(true);
        SettingsMenuGO.SetActive(false);
        RemuseButton.Select();
    }

    public void Desactivate()
    {
        Debug.Log("Desactivate");

        gameObject.SetActive(false);
    }

    public void Quit() => Application.Quit();

    public void Resume()
    {
        Debug.Log("Resume Game");
        _SFXManager.ChangeState("Select");

        Desactivate();
        _playerUI.TogglePauseMenu(true);
    }

    public void GoLeave()
    {
        _SFXManager.ChangeState("Back");
        PauseMenuGO.SetActive(false);
        LeaveMenuGO.SetActive(true);
        _state = State.LeaveGameState;
        Debug.Log("Go leave");
    }

    private void BackToMenu()
    {
        _SFXManager.ChangeState("Back");
        LeaveMenuGO.SetActive(false);
        PauseMenuGO.SetActive(true);
        _state = State.PauseionState;
        Debug.Log("Back to menu");
    }

    public void GoSettings()
    {
        _SFXManager.ChangeState("Navigate");
        PauseMenuGO.SetActive(false);
        SettingsMenuGO.SetActive(true);
        GoToAudio();
        masterMusicSlider.Select();
        _state = State.SettingState;
    }

    private void BackOfSettings()
    {
        _SFXManager.ChangeState("Back");
        SettingsMenuGO.SetActive(false);
        GoToAudio();
        PauseMenuGO.SetActive(true);
        RemuseButton.Select();
        _state = State.PauseionState;
    }

    private void GoToControle()
    {
        _SFXManager.ChangeState("Navigate");

        if (!ControleGo.activeSelf)
        {
            horizontalSlider.Select();
            AudioGo.SetActive(false);
            ControleGo.SetActive(true);
        }
    }

    private void GoToAudio()
    {
        _SFXManager.ChangeState("Navigate");

        if (!AudioGo.activeSelf)
        {
            masterMusicSlider.Select();
            ControleGo.SetActive(false);
            AudioGo.SetActive(true);
        }
    }

    public void OnAButton()
    {
        switch (_state)
        {
            case State.LeaveGameState:
                GameManager.instance.Quit();
                break;
            case State.SettingState:
                break;
            default:
                break;
        }
        Debug.Log("Press A");
    }

    public void OnBButton()
    {
        switch (_state)
        {
            case State.ResumeState:
                Resume();
                break;
            case State.PauseionState:
                Resume();
                break;
            case State.LeaveGameState:
                BackToMenu();
                break;
            case State.SettingState:
                BackOfSettings();
                break;
            default:
                break;
        }
        Debug.Log("Press B");
    }

    public void OnYButton()
    {
        switch (_state)
        {
            case State.PauseionState:
                GoSettings();
                break;
            case State.LeaveGameState:
                break;
            case State.SettingState:
                break;
            default:
                break;
        }
        Debug.Log("Press Y");
    }

    public void OnRB()
    {
        switch (_state)
        {
            case State.PauseionState:
                break;
            case State.LeaveGameState:
                break;
            case State.SettingState:
                GoToControle();
                break;
            default:
                break;
        }
        Debug.Log("Press Y");
    }

    public void OnLB()
    {
        switch (_state)
        {
            case State.PauseionState:
                break;
            case State.LeaveGameState:
                break;
            case State.SettingState:
                GoToAudio();
                break;
            default:
                break;
        }
        Debug.Log("Press Y");
    }

    public void OnLook(InputValue value)
    {
        Debug.Log("Press Look");
    }

    public void OnMovement(InputValue value)
    {
        Debug.Log("Press Movement");
    }
}
