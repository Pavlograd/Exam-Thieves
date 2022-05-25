using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using StaticClassSettingsGame;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIHome : MonoBehaviour
{

    private enum State
    {
        PressAnyKeyState,
        ConnectionState,
        InQueueState,
        SettingState,
        LeaveGameState,
        Tutorial,
        Credit
    }

    [SerializeField] private GameObject HomeMenuGO;
    [SerializeField] private GameObject ConnectMenuGO;
    [SerializeField] private GameObject LeaveMenuGO;
    [SerializeField] private GameObject QueueMenuGO;
    [SerializeField] private GameObject SettingsMenuGO;
    [SerializeField] private GameObject SettingsAudioGo;
    [SerializeField] private GameObject SettingsControleGo;
    [SerializeField] private TMP_Text SettingTitle;
    [Header("Tutorial")]
    [SerializeField] private Sprite[] listImageTutorial;
    [SerializeField] private GameObject tutorialGO;
    [SerializeField] private Image imageTutorial;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject backButton;
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

    [Header("Crédit")]
    [SerializeField] private GameObject creditGo;
    [SerializeField] private GameObject leaveButtonCredit;
    private credit _credit;
    
    
    private int _tutorialInt = 0;

    private bool allowChange = true;

    private void Start()
    {
        horizontalSlider.value = PlayerSettings.gamePadSensitivityX;
        verticalSlider.value = PlayerSettings.gamePadSensitivityY;

        inverseAxisToggle.isOn = PlayerSettings.inverseAxis;

        masterMusicSlider.value = PlayerSettings.masterMusicLevel;
        musicMusicSlider.value = PlayerSettings.musicMusicLevel;
        fxMusicSlider.value = PlayerSettings.fxMusicLevel;
        _credit = creditGo.GetComponentInChildren<credit>();
    }

    private State _state = State.PressAnyKeyState;
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
    private void Quit() => Application.Quit();
    private void PressAnyKey()
    {
        _SFXManager.ChangeState("Select");
        HomeMenuGO.SetActive(false);
        ConnectMenuGO.SetActive(true);
        ShowTuTo();
        StartCoroutine(waitTutoNext());
        Debug.Log("Press Any Key");
    }

    IEnumerator waitTutoNext()
    {
        yield return new WaitForSeconds(2);
        nextButton.SetActive(true);
    }

    private void GoLeave()
    {
        _SFXManager.ChangeState("Back");
        ConnectMenuGO.SetActive(false);
        LeaveMenuGO.SetActive(true);
        _state = State.LeaveGameState;
        Debug.Log("Go leave");
    }

    private void BackToMenu()
    {
        _SFXManager.ChangeState("Back");
        LeaveMenuGO.SetActive(false);
        ConnectMenuGO.SetActive(true);
        _state = State.ConnectionState;
        Debug.Log("Back to menu");
    }

    private void JoinQueue()
    {
        _SFXManager.ChangeState("Select");
        _matchmaker.StartJoinQueu(false);
        ConnectMenuGO.SetActive(false);
        QueueMenuGO.SetActive(true);
        _state = State.InQueueState;
    }

    private void LeaveQueue()
    {
        _SFXManager.ChangeState("Back");
        _matchmaker.LeaveQueue();
        QueueMenuGO.SetActive(false);
        ConnectMenuGO.SetActive(true);
        _state = State.ConnectionState;
    }

    private void GoSettings()
    {
        _SFXManager.ChangeState("Navigate");
        ConnectMenuGO.SetActive(false);
        SettingsMenuGO.SetActive(true);
        masterMusicSlider.Select();
        _state = State.SettingState;
    }

    private void BackOfSettings()
    {
        _SFXManager.ChangeState("Back");
        SettingsMenuGO.SetActive(false);
        GoToAudio();
        ConnectMenuGO.SetActive(true);
        _state = State.ConnectionState;
    }

    private void GoToControle()
    {
        _SFXManager.ChangeState("Navigate");
        if (!SettingsControleGo.activeSelf)
        {
            SettingTitle.text = "CONTROLS";
            horizontalSlider.Select();
            SettingsAudioGo.SetActive(false);
            SettingsControleGo.SetActive(true);
        }
    }

    private void GoToAudio()
    {
        _SFXManager.ChangeState("Navigate");
        if (!SettingsAudioGo.activeSelf)
        {
            SettingTitle.text = "AUDIO";
            masterMusicSlider.Select();
            SettingsControleGo.SetActive(false);
            SettingsAudioGo.SetActive(true);
        }
    }

    private void PassTuto()
    {
        if (!nextButton.activeSelf) return;
        _tutorialInt++;
        if (_tutorialInt >= listImageTutorial.Length)
            SkipTuto();
        else
            displaySpriteTuto();
    }

    private void BackTuto()
    {
        if (_tutorialInt <= 0) return;
        _tutorialInt--;
        displaySpriteTuto();
    }

    private void displaySpriteTuto()
    {
        imageTutorial.sprite = listImageTutorial[_tutorialInt];
        if (_tutorialInt > 0)
            backButton.SetActive(true);
        else
            backButton.SetActive(false);
    }

    private void SkipTuto()
    {
        _SFXManager.ChangeState("Navigate");
        _tutorialInt = 0;
        tutorialGO.SetActive(false);
        _state = State.ConnectionState;
    }

    private void ShowTuTo()
    {
        _state = State.Tutorial;
        tutorialGO.SetActive(true);
        displaySpriteTuto();
    }

    private void ShowCredit()
    {
        _SFXManager.ChangeState("Back");
        _state = State.Credit;
        ConnectMenuGO.SetActive(false);
        creditGo.SetActive(true);
        _credit.StartMotionCrédit();
    }
    
    private void BackOfCredit()
    {
        if (!leaveButtonCredit.activeSelf) return;
        _SFXManager.ChangeState("Back");
        _state = State.ConnectionState;
        creditGo.SetActive(false);
        ConnectMenuGO.SetActive(true);
    }
    private void SkipCredit()
    {
        _SFXManager.ChangeState("Navigate");
        _state = State.ConnectionState;
        _credit.Stop();
        creditGo.SetActive(false);
        ConnectMenuGO.SetActive(true);
    }
    public void OnAButton()
    {
        switch (_state)
        {
            case State.PressAnyKeyState:
                PressAnyKey();
                break;
            case State.ConnectionState:
                Debug.Log("Start Search");
                JoinQueue();
                break;
            case State.InQueueState:
                Debug.Log("In join queue");
                break;
            case State.LeaveGameState:
                Quit();
                break;
            case State.SettingState:
                break;
            case State.Tutorial:
                PassTuto();
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
            case State.PressAnyKeyState:
                PressAnyKey();
                break;
            case State.ConnectionState:
                GoLeave();
                break;
            case State.InQueueState:
                LeaveQueue();
                break;
            case State.LeaveGameState:
                BackToMenu();
                break;
            case State.SettingState:
                BackOfSettings();
                break;
            case State.Tutorial:
                BackTuto();
                break;
            case State.Credit:
                BackOfCredit();
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
            case State.PressAnyKeyState:
                PressAnyKey();
                break;
            case State.ConnectionState:
                GoSettings();
                break;
            case State.InQueueState:
                break;
            case State.LeaveGameState:
                break;
            case State.SettingState:
                break;
            case State.Tutorial:
                //SkipTuto();
                break;
            case State.Credit:
                SkipCredit();
                break;
            default:
                break;
        }
        Debug.Log("Press Y");
    }

    public void OnXButton()
    {
        switch (_state)
        {
            case State.ConnectionState:
                ShowTuTo();
                break;
            default:
                break;
        }
    }

    public void OnRB()
    {
        switch (_state)
        {
            case State.PressAnyKeyState:
                break;
            case State.ConnectionState:
                break;
            case State.InQueueState:
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
            case State.PressAnyKeyState:
                break;
            case State.ConnectionState:
                break;
            case State.InQueueState:
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

    public void OnStart()
    {
        switch (_state)
        {
            case State.ConnectionState:
                ShowCredit();
                break;
            default:
                break;
        }
    }
}
