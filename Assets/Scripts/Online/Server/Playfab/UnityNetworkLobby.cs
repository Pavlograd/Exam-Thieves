using System;
using Mirror;
using PlayFab.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnityNetworkLobby : NetworkBehaviour
{
    [Header("UI")] 
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private String[] playerName = new string[2];
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[2];
    [SerializeField] private Image[] playerCharacterSelectImageList = new Image[2];
    [SerializeField] private TMP_Text readyBtnText;
    [SerializeField] private TMP_Text[] listDifficultyTexts = new TMP_Text[3];

    public PlayerData[] _playerData = new PlayerData[4];
    
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    
    [SyncVar(hook = nameof(HandleIsReadyChanged))]
    public bool isReady;

    [SyncVar(hook = nameof(HandleChooseDifficulty))]
    public bool chooseDifficultyState = false;
    
    [SyncVar(hook = nameof(HandleTypeChanged))]
    public string type;

    [SyncVar(hook = nameof(HandleCLientLeft))]
    public bool oppenentLeave = false;

    [SyncVar(hook = nameof(HandleTimer))] 
    public int timer = -1;

    [SyncVar(hook = nameof(HandleTimer))]
    public int playerCharacterSelectImage;
    
    [SyncVar(hook = nameof(HandleTimer))]
    public int _difficultyInt = 1;

    [SyncVar] public int _skinColor;
    [SyncVar] public int _hairColor;

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleChooseDiff(int oldValue, int newValue) => UpdateDisplay();
    public void HandleIsReadyChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleTypeChanged(string oldValue, string newValue) => UpdateDisplay();

    public void HandleTimer(int oldValue, int newValue) => UpdateDisplay();
    public void HandleCLientLeft(bool oldValue, bool newValue) => ClientLeft();
    
    public void HandleChooseDifficulty(bool oldValue, bool newValue) => ChooseDifficulty();
    
    [SyncVar]
    public bool isLeader;
    

    private UnityNetworkServer room;
    private UnityNetworkServer Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }

            return room = NetworkManager.singleton as UnityNetworkServer;
        }
    }
    
    public override void OnStartAuthority()
    {
        lobbyUI.SetActive(true);
    }

    public override void OnStopAuthority()
    {
        Debug.Log(DisplayName + " loose autgority");
    }

    [Command]
    private void CmdSetDisplayName(string _name)
    {
        DisplayName = _name;
    }
    
    [Command]
    public void CmdReadyUp()
    {
        isReady = !isReady;
        Room.NotifyPlayerOfReadyState();
    }

    public void ChooseType(int typeNb)
    {
        
        CmdChangeType(typeNb);
        if (isReady)
            CmdReadyUp();
    }

    public void ChooseColor(int skin, int hair)
    {
        CmdChangeColor(skin, hair);
    }
    
    [Command]
    private void CmdChangeType(int typeNb)
    {
        playerCharacterSelectImage = typeNb;
        type = _playerData[playerCharacterSelectImage].nameCharacters;
        UpdateDisplay();
    }

    [Command]
    private void CmdChangeColor(int skin, int hair)
    {
        _skinColor = skin;
        _hairColor = hair;
    }

    [Command]
    public void CmdStartGame()
    {
        Room.StartGame();
    }


    public override void OnStartClient()
    {
        Room._RoomPlayers.Add(this);
        if (!hasAuthority)
        {
            this.gameObject.SetActive(false);   
        }
        else
        {
            CmdSetDisplayName(isLeader ? playerName[0] : playerName[1]); 
            ChooseType(0);
        }
        Debug.Log("Room: " + Room._RoomPlayers.Count);
        UpdateDisplay();
    }

    public void ChangeDiff(bool right)
    {
        CmdChangeDiff(right);
    }

    [Command]
    private void CmdChangeDiff(bool right)
    {
        if (right)
            _difficultyInt = _difficultyInt == listDifficultyTexts.Length - 1 ? 0 : _difficultyInt + 1;
        else
            _difficultyInt = _difficultyInt == 0 ? listDifficultyTexts.Length - 1 : _difficultyInt - 1;
        //UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (UnityNetworkLobby player in Room._RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }
        
        if (!chooseDifficultyState)
        {
            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting For Player...";
                playerReadyTexts[i].text = "Waiting For Player...";
                //normalement ce sera . sprite
                playerCharacterSelectImageList[i].sprite = null;
                playerCharacterSelectImageList[i].color = new Color(0,0,0,0);
                //playerTypeTexts[i].text = "Choose Type";
            }

            for (int i = 0; i < Room._RoomPlayers.Count; i++)
            {
                //playerTypeTexts[i].text = Room._RoomPlayers[i].type.Length == 0 ? "Choose Type" : Room._RoomPlayers[i].type;
                playerNameTexts[i].text = isLeader ? playerName[0] : playerName[1];
                if (timer != -1)
                {
                    readyBtnText.text = "GAME START IN " + timer.ToString();
                } else 
                    readyBtnText.text = isReady ? "NOT READY" : "READY";
                playerReadyTexts[i].text = Room._RoomPlayers[i].DisplayName;
                playerReadyTexts[i].color = Room._RoomPlayers[i].isReady ?
                    Color.green:
                    Color.red;
                playerCharacterSelectImageList[i].color = Room._RoomPlayers[i]._playerData[Room._RoomPlayers[i].playerCharacterSelectImage].playerImage.color;
                playerCharacterSelectImageList[i].sprite = Room._RoomPlayers[i]._playerData[Room._RoomPlayers[i].playerCharacterSelectImage].playerImage.sprite;
            }
        }
        else
        {
            _difficultyInt = Room._RoomPlayers[0].isLeader
                ? Room._RoomPlayers[0]._difficultyInt
                : Room._RoomPlayers[1]._difficultyInt;
            Debug.Log("Debug la diff " + _difficultyInt);
            for (int i = 0; i < listDifficultyTexts.Length; i++)
            {
                if (i == _difficultyInt)
                    listDifficultyTexts[i].color = Color.yellow;
                else
                    listDifficultyTexts[i].color = Color.white;
            }
        }
    }
    
    private void ClientLeft()
    {
        gameObject.GetComponentInChildren<UIPlayerLobby>().OpponentLeaveGame();
    }
    
    private void ChooseDifficulty()
    {
        gameObject.GetComponentInChildren<UIPlayerLobby>().ChooseDifficulty(isLeader);
    }

    public void HandleReadyToStart(bool ready)
    {
        //Si tout le monde est ready on active le button pour start la game
       /* if (type != null)
            _buttonReady.interactable = true;*/
    }
}
