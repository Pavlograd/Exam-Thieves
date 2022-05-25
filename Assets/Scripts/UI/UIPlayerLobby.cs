using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public struct BasicColorPlayer
{
    public int hairColor;
    public int skinColor;
}

public class UIPlayerLobby : MonoBehaviour
{
    private enum State
    {
        HomeLobby,
        CustomizePlayer,
        ChooseDifficulty,
        WaitDifficulty,
        LeaveGame,
        OpponentLeaveGame,
    }

    [SerializeField] private GameObject homeLobbyGO;
    [SerializeField] private GameObject leaveLobbyGo;
    [SerializeField] private GameObject opponentLeaveGameGO;
    [SerializeField] private GameObject waitChooseDiffGO;
    [SerializeField] private GameObject chooseDiffGO;
    [SerializeField] private GameObject switchDiffGO;
    [SerializeField] private GameObject setDiffGO;
    [SerializeField] private GameObject customizationGO;
    [SerializeField] private TMP_Text readyBtnText; //Ready / Not Ready;
    [SerializeField] private Image[] listPlayerCadre = new Image[4];
    [SerializeField] private TMP_Text playerNameTxt;
    [SerializeField] private TMP_Text playerDescTxt;
    [SerializeField] private GameObject[] model3DPlayer = new GameObject[4];
    [SerializeField] private GameObject model3DPlayerGo;
    [SerializeField] private Transform postition3DModelHomeMenu;
    [SerializeField] private TMP_Text titleDiff;
    [Header("Customization")]
    [SerializeField] private Transform postition3DModelCustomizationMenu;
    [SerializeField] private Image[] skinContourUI;
    [SerializeField] private Image[] hairContourUI;
    [SerializeField] private GameObject selectorSkin;
    [SerializeField] private GameObject selectorHair;

    [Header("Audio")]
    [SerializeField] private SFXManager _SFXManager;

    private State _state = State.HomeLobby;
    private UnityNetworkLobby _unityNetworkLobby;

    private int _cadrePlayerInt = 0;
    private int _difficultyInt = 1;
    private int _tutorialInt = 0;
    private int selectedCusto = 0;

    private bool allowChange = true;

    private int[] _skinColorint = new int[4] { 0, 0, 0, 0 };
    private int[] _hairColorint = new int[4] { 0, 6, 1, 6 };

    private void Start()
    {
        _unityNetworkLobby = gameObject.GetComponentInParent<UnityNetworkLobby>();
        listPlayerCadre[_cadrePlayerInt].color = Color.yellow;
        playerNameTxt.text = _unityNetworkLobby._playerData[_cadrePlayerInt].nameCharacters;
        playerDescTxt.text = _unityNetworkLobby._playerData[_cadrePlayerInt].descCharacters;
    }

    public void ChooseDifficulty(bool isLeader)
    {
        _SFXManager.ChangeState("Select");
        homeLobbyGO.SetActive(false);
        model3DPlayerGo.SetActive(false);
        chooseDiffGO.SetActive(true);
        if (isLeader)
        {
            _state = State.ChooseDifficulty;
        }
        else
        {
            _state = State.WaitDifficulty;
            setDiffGO.SetActive(false);
            switchDiffGO.SetActive(false);
            waitChooseDiffGO.SetActive(true);
            titleDiff.text = "The other player is choosing the difficulty";
        }
    }

    private void ReadyUp()
    {
        _SFXManager.ChangeState("Select");
        _unityNetworkLobby.CmdReadyUp();
    }

    private void ChangePlayer(bool right)
    {
        _SFXManager.ChangeState("Select");
        listPlayerCadre[_cadrePlayerInt].color = Color.white;
        if (right)
            _cadrePlayerInt = _cadrePlayerInt == listPlayerCadre.Length - 1 ? 0 : _cadrePlayerInt + 1;
        else
            _cadrePlayerInt = _cadrePlayerInt == 0 ? listPlayerCadre.Length - 1 : _cadrePlayerInt - 1;
        listPlayerCadre[_cadrePlayerInt].color = Color.yellow;
        ChangePlayerModel3D();
        playerNameTxt.text = _unityNetworkLobby._playerData[_cadrePlayerInt].nameCharacters;
        playerDescTxt.text = _unityNetworkLobby._playerData[_cadrePlayerInt].descCharacters;
        _unityNetworkLobby.ChooseType(_cadrePlayerInt);
        SetServerColor();
    }

    private void ChangeSkinColor(bool right)
    {
        _SFXManager.ChangeState("Select");
        skinContourUI[_skinColorint[_cadrePlayerInt]].color = Color.white;
        //skinContourUI[_skinColorint[_cadrePlayerInt]].gameObject.SetActive(false);
        if (right)
            _skinColorint[_cadrePlayerInt] = _skinColorint[_cadrePlayerInt] == skinContourUI.Length - 1 ? 0 : _skinColorint[_cadrePlayerInt] + 1;
        else
            _skinColorint[_cadrePlayerInt] = _skinColorint[_cadrePlayerInt] == 0 ? skinContourUI.Length - 1 : _skinColorint[_cadrePlayerInt] - 1;
        skinContourUI[_skinColorint[_cadrePlayerInt]].color = Color.yellow;
        //skinContourUI[_skinColorint[_cadrePlayerInt]].gameObject.SetActive(true);
        ChangeColorModel3D();
    }

    private void ChangeHairColor(bool right)
    {
        _SFXManager.ChangeState("Select");
        hairContourUI[_hairColorint[_cadrePlayerInt]].color = Color.white;
        //hairContourUI[_hairColorint[_cadrePlayerInt]].gameObject.SetActive(false);
        if (right)
            _hairColorint[_cadrePlayerInt] = _hairColorint[_cadrePlayerInt] == hairContourUI.Length - 1 ? 0 : _hairColorint[_cadrePlayerInt] + 1;
        else
            _hairColorint[_cadrePlayerInt] = _hairColorint[_cadrePlayerInt] == 0 ? hairContourUI.Length - 1 : _hairColorint[_cadrePlayerInt] - 1;
        hairContourUI[_hairColorint[_cadrePlayerInt]].color = Color.yellow;
        //hairContourUI[_hairColorint[_cadrePlayerInt]].gameObject.SetActive(true);
        ChangeColorModel3D();
    }

    private void ResetContour()
    {
        foreach (Image contour in hairContourUI)
        {
            contour.color = Color.white;
            //contour.gameObject.SetActive(false);
        }

        foreach (Image contour in skinContourUI)
        {
            contour.color = Color.white;
            //contour.gameObject.SetActive(false);
        }
    }

    private void SetSelectedConbtour()
    {
        ResetContour();
        foreach (GameObject t in model3DPlayer)
        {
            if (t.activeSelf)
            {
                BasicColorPlayer basicColorPlayer = t.GetComponent<PlayerCusto>().GetSkinBasicColorPlayerColor();
                hairContourUI[basicColorPlayer.hairColor].color = Color.yellow;
                skinContourUI[basicColorPlayer.skinColor].color = Color.yellow;
                //skinContourUI[basicColorPlayer.skinColor].gameObject.SetActive(true);
                //hairContourUI[basicColorPlayer.hairColor].gameObject.SetActive(true);
                break;
            }
        }
    }

    private void SetServerColor()
    {
        _unityNetworkLobby.ChooseColor(_skinColorint[_cadrePlayerInt], _hairColorint[_cadrePlayerInt]);
    }

    private void ChangeColorModel3D()
    {
        foreach (GameObject t in model3DPlayer)
        {
            if (t.activeSelf)
            {
                t.GetComponent<PlayerCusto>().ChangeColor(_skinColorint[_cadrePlayerInt], _hairColorint[_cadrePlayerInt]);
                break;
            }
        }
        SetServerColor();
    }

    private void ChangePlayerModel3D()
    {
        for (int i = 0; i < model3DPlayer.Length; i++)
        {
            model3DPlayer[i].SetActive(i == _cadrePlayerInt);
        }
    }

    private void ChangeDifficultyBtn(bool right)
    {
        _SFXManager.ChangeState("Select");
        _unityNetworkLobby.ChangeDiff(right);
    }

    private void GoToLeaveGame()
    {
        _SFXManager.ChangeState("Back");
        if (_unityNetworkLobby.isReady)
            _unityNetworkLobby.CmdReadyUp();
        homeLobbyGO.SetActive(false);
        model3DPlayerGo.SetActive(false);
        leaveLobbyGo.SetActive(true);
        _state = State.LeaveGame;
    }

    private void GoToHomeLobby()
    {
        _SFXManager.ChangeState("Back");
        leaveLobbyGo.SetActive(false);
        customizationGO.SetActive(false);
        homeLobbyGO.SetActive(true);
        model3DPlayerGo.transform.localPosition = postition3DModelHomeMenu.localPosition;
        model3DPlayerGo.transform.localRotation = postition3DModelHomeMenu.localRotation;
        model3DPlayerGo.transform.localScale = postition3DModelHomeMenu.localScale;
        model3DPlayerGo.SetActive(true);
        _unityNetworkLobby.UpdateDisplay();
        _state = State.HomeLobby;
    }

    private void LeaveGame()
    {
        _SFXManager.ChangeState("Back");
        NetworkClient.Disconnect();
    }

    public void OpponentLeaveGame()
    {
        _SFXManager.ChangeState("Back");
        _state = State.OpponentLeaveGame;
        homeLobbyGO.SetActive(false);
        chooseDiffGO.SetActive(false);
        leaveLobbyGo.SetActive(false);
        customizationGO.SetActive(false);
        model3DPlayerGo.SetActive(false);
        opponentLeaveGameGO.SetActive(true);
    }

    private void GoToCutomization()
    {
        _SFXManager.ChangeState("Navigate");
        _state = State.CustomizePlayer;
        homeLobbyGO.SetActive(false);
        customizationGO.SetActive(true);
        SetSelectedConbtour();
        model3DPlayerGo.transform.localPosition = postition3DModelCustomizationMenu.localPosition;
        model3DPlayerGo.transform.localRotation = postition3DModelCustomizationMenu.localRotation;
        model3DPlayerGo.transform.localScale = postition3DModelCustomizationMenu.localScale;
    }

    private void Reset3DModel()
    {
        _SFXManager.ChangeState("Back");
        foreach (GameObject t in model3DPlayer)
        {
            if (t.activeSelf)
            {
                PlayerCusto playerCusto = t.GetComponent<PlayerCusto>();
                playerCusto.Reset();
                BasicColorPlayer basicColorPlayer = playerCusto.GetSkinBasicColorPlayerColor();
                _skinColorint[_cadrePlayerInt] = basicColorPlayer.skinColor;
                _hairColorint[_cadrePlayerInt] = basicColorPlayer.hairColor;
                break;
            }
        }
        SetSelectedConbtour();
        SetServerColor();
    }
    private void DisplaySelector()
    {
        if (selectedCusto == 0)
        {
            selectorHair.SetActive(false);
            selectorSkin.SetActive(true);
        }
        else
        {
            selectorSkin.SetActive(false);
            selectorHair.SetActive(true);
        }
    }

    private void NavigationCusto(Vector2 value)
    {
        if (value.Equals(Vector2.zero))
        {
            allowChange = true;
            return;
        }
        if (value.y != 0)
        {
            if (value.y == 1)
            {
                selectedCusto = 0;
            } else if (value.y == -1)
                selectedCusto = 1;
            DisplaySelector();
        }
        else if (allowChange)
        {
            if (value.Equals(Vector2.right))
            {
                if (selectedCusto == 0)
                    ChangeSkinColor(true);
                else
                    ChangeHairColor(true);   
            }
            else if (value.Equals(Vector2.left))
                if (selectedCusto == 0)
                    ChangeSkinColor(false);
                else
                    ChangeHairColor(false);
            allowChange = false;
        }
    }
    
    public void OnNavigate(InputValue value)
    {
        switch (_state)
        {
            case State.CustomizePlayer:
                NavigationCusto(value.Get<Vector2>());
                break;
            default:
                break;
        }
    }

    public void OnAButton()
    {
        switch (_state)
        {
            case State.HomeLobby:
                ReadyUp();
                break;
            case State.CustomizePlayer:
                GoToHomeLobby();
                break;
            case State.ChooseDifficulty:
                _unityNetworkLobby.CmdStartGame();
                break;
            case State.LeaveGame:
                Debug.Log("Leave the room methode");
                LeaveGame();
                break;
            case State.OpponentLeaveGame:
                LeaveGame();
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
            case State.HomeLobby:
                GoToLeaveGame();
                break;
            case State.CustomizePlayer:
                GoToHomeLobby();
                break;
            case State.ChooseDifficulty:
                Debug.Log("In join queue");
                break;
            case State.LeaveGame:
                GoToHomeLobby();
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
            case State.HomeLobby:
                GoToCutomization();
                break;
            case State.CustomizePlayer:
                Reset3DModel();
                break;
            case State.ChooseDifficulty:
                Debug.Log("In join queue");
                break;
            default:
                break;
        }
        Debug.Log("Press T");
    }

    public void OnRB()
    {
        switch (_state)
        {
            case State.HomeLobby:
                ChangePlayer(true);
                break;
            case State.CustomizePlayer:
                //ChangeSkinColor(true);
                break;
            case State.ChooseDifficulty:
                ChangeDifficultyBtn(true);
                break;
            default:
                break;
        }
        Debug.Log("Press RT");
    }

    public void OnLB()
    {
        switch (_state)
        {
            case State.HomeLobby:
                ChangePlayer(false);
                break;
            case State.CustomizePlayer:
                //ChangeSkinColor(false);
                break;
            case State.ChooseDifficulty:
                ChangeDifficultyBtn(false);
                break;
            default:
                break;
        }
        Debug.Log("Press LT");
    }

    public void OnLT()
    {
        switch (_state)
        {
            case State.CustomizePlayer:
                //ChangeHairColor(false);
                break;
            default:
                break;
        }
        Debug.Log("Press LT");
    }

    public void OnRT()
    {
        switch (_state)
        {
            case State.CustomizePlayer:
                //ChangeHairColor(true);
                break;
            default:
                break;
        }
        Debug.Log("Press LT");
    }
}
