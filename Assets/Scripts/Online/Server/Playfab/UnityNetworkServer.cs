using System.Collections;

namespace PlayFab.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Mirror;
    using PlayFab;
    using UnityEngine.Events;
    using PlayFab.MultiplayerAgent.Model;

    public class UnityNetworkServer : NetworkManager
    {
        public static UnityNetworkServer Instance { get; private set; }
        private List<ConnectedPlayer> _connectedPlayers;
        public PlayerEvent OnPlayerAdded = new PlayerEvent();
        public PlayerEvent OnPlayerRemoved = new PlayerEvent();
        private Coroutine _timerCoroutine;
        private bool onGame = false;
        [SerializeField] private DifficultyData[] listDiff;

        [SerializeField] private GameObject roomPlayerPrefab = null;

        public List<UnityNetworkConnection> Connections
        {
            get { return _connections; }
            private set { _connections = value; }
        }
        private List<UnityNetworkConnection> _connections = new List<UnityNetworkConnection>();
        public List<UnityNetworkLobby> _RoomPlayers { get; } = new List<UnityNetworkLobby>();

        public class PlayerEvent : UnityEvent<string> { }



        public void ChooseDifficulty()
        {
            Debug.Log("Choose Difficulty");
            for (int i = _RoomPlayers.Count - 1; i >= 0; i--)
            {
                _RoomPlayers[i].chooseDifficultyState = true;
            }
        }

        private void LoadMapProcedural()
        {
            GameObject procedural = GameObject.FindWithTag("Procedural");
            Debug.Log(procedural != null);
            procedural.GetComponent<ProceduralGeneration>().enabled = true;
        }
        public void StartGame()
        {
            GameManager.instance._difficulty = _RoomPlayers[0]._difficultyInt;
            DifficultySettings.datas = listDiff[GameManager.instance._difficulty];
            Debug.LogWarning("Nombre de Garde" + DifficultySettings.datas.nbrGuards);
            Debug.Log("server Start game");
            Debug.Log("In the new scene");
            Debug.Log(_RoomPlayers.Count);
            GameManager.instance.StartTimer();
            LoadMapProcedural();
            onGame = true;
            for (int i = _RoomPlayers.Count - 1; i >= 0; i--)
            {
                Debug.Log("in boucle");
                Debug.Log(_RoomPlayers[i]._difficultyInt);
                NetworkConnection conn = _RoomPlayers[i].connectionToClient;
                GameObject gameplayerInstance = Instantiate(playerPrefab);
                Debug.Log("Serveur " + _RoomPlayers[i]._skinColor + " hair " + _RoomPlayers[i]._hairColor);
                gameplayerInstance.GetComponent<PlayerSetup>().SetType(_RoomPlayers[i].type, _RoomPlayers[i]._skinColor, _RoomPlayers[i]._hairColor);
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance);
            }
        }

        public override void ServerChangeScene(string nameScene)
        {

            base.ServerChangeScene(nameScene);
        }

        // Use this for initialization
        public override void Awake()
        {
            base.Awake();
            base.LateUpdate();
            Instance = this;
            NetworkServer.RegisterHandler<ReceiveAuthenticateMessage>(OnReceiveAuthenticate);
            NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);

            //_netManager.transport.port = Port;
        }


        void OnCreateCharacter(NetworkConnection conn, CreateCharacterMessage message)
        {
            // playerPrefab is the one assigned in the inspector in Network
            // Manager but you can use different prefabs per race for example
            GameObject gameobject = Instantiate(playerPrefab);
            Debug.Log(message);
            /* // Apply data from the message however appropriate for your game
             // Typically Player would be a component you write with syncvars or properties
             Player player = gameobject.GetComponent();
             player.hairColor = message.hairColor;
             player.eyeColor = message.eyeColor;
             player.name = message.name;
             player.race = message.race;*/

            // call this to use this gameobject as the primary controller
            NetworkServer.AddPlayerForConnection(conn, gameobject);
        }

        public void CreatePlayer()
        {
            // you can send the message here, or wherever else you want
            CreateCharacterMessage characterMessage = new CreateCharacterMessage
            {
                nameCharacter = "Zepoulpe",
                HairColor = Color.blue
            };

            NetworkClient.Send(characterMessage);
        }

        public override void OnServerAddPlayer(NetworkConnection nconn)
        {
            bool isLeader = _RoomPlayers.Count == 0;
            Debug.Log(isLeader);
            GameObject lobbyPlayerInstance = Instantiate(roomPlayerPrefab);
            lobbyPlayerInstance.GetComponent<UnityNetworkLobby>().isLeader = isLeader;
            NetworkServer.AddPlayerForConnection(nconn, lobbyPlayerInstance);

            _RoomPlayers.Add(lobbyPlayerInstance.GetComponent<UnityNetworkLobby>());
            Debug.Log("Player was added");
        }

        public void StartListen()
        {
            Debug.Log("Real Start");
            NetworkServer.Listen(maxConnections);
        }

        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            NetworkServer.Shutdown();
        }

        private void Stop()
        {
            Application.Quit();
        }

        private void OnReceiveAuthenticate(NetworkConnection nconn, ReceiveAuthenticateMessage message)
        {
            var conn = _connections.Find(c => c.ConnectionId == nconn.connectionId);
            Debug.Log("Quelqu'un se connecte");
            if (conn != null)
            {
                conn.PlayFabId = message.PlayFabId;
                conn.IsAuthenticated = true;
                OnPlayerAdded.Invoke(message.PlayFabId);
            }
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);

            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }
            Debug.LogWarning("Client Connected");
            var uconn = _connections.Find(c => c.ConnectionId == conn.connectionId);
            if (uconn == null)
            {
                _connections.Add(new UnityNetworkConnection()
                {
                    Connection = conn,
                    ConnectionId = conn.connectionId,
                    LobbyId = PlayFabMultiplayerAgentAPI.SessionConfig.SessionId
                });
            }
        }

        public override void OnServerError(NetworkConnection conn, Exception errorCode)
        {
            base.OnServerError(conn, errorCode);

            Debug.Log(string.Format("Unity Network Connection Status: code - {0}", errorCode));
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            Debug.LogWarning("Client disconnected");
            var uconn = _connections.Find(c => c.ConnectionId == conn.connectionId);
            if (uconn != null)
            {
                if (!string.IsNullOrEmpty(uconn.PlayFabId))
                {
                    OnPlayerRemoved.Invoke(uconn.PlayFabId);
                }
                _connections.Remove(uconn);
                if (conn.identity != null)
                {
                    _RoomPlayers.Remove(conn.identity.GetComponent<UnityNetworkLobby>());
                    Debug.Log(_RoomPlayers.Count);
                    if (_RoomPlayers.Count == 1 && !onGame)
                        _RoomPlayers[0].oppenentLeave = true;
                }
                Stop();
            }
            base.OnServerDisconnect(conn);
        }

        public void NotifyPlayerOfReadyState()
        {
            /*foreach (UnityNetworkLobby player in _RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }*/
            //TTTTT
            if (IsReadyToStart() && _timerCoroutine == null)
            {
                //Coruôutine sync pour lancer le jeu
                _timerCoroutine = StartCoroutine(timer());
            }
            else if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
                foreach (UnityNetworkLobby roomPlayer in _RoomPlayers)
                {
                    roomPlayer.timer = -1;
                }
            }
        }

        IEnumerator timer()
        {
            int i = 5;
            while (i > 0)
            {
                foreach (UnityNetworkLobby roomPlayer in _RoomPlayers)
                {
                    roomPlayer.timer = i;
                }
                yield return new WaitForSeconds(1f);
                i = i - 1;
                yield return null;
            }
            ChooseDifficulty();
            //StartGame();
            yield break;
        }

        private bool IsReadyToStart()
        {
            if (numPlayers < maxConnections)
            {
                return false;
            }

            foreach (UnityNetworkLobby player in _RoomPlayers)
            {
                if (!player.isReady)
                    return false;
            }

            return true;
        }


    }

    [Serializable]
    public class UnityNetworkConnection
    {
        public bool IsAuthenticated;
        public string PlayFabId;
        public string LobbyId;
        public int ConnectionId;
        public NetworkConnection Connection;
    }

    public class CustomGameServerMessageTypes
    {
        public const short ReceiveAuthenticate = 900;
        public const short ShutdownMessage = 901;
        public const short MaintenanceMessage = 902;
    }

    public struct CreateCharacterMessage : NetworkMessage
    {
        public string nameCharacter;
        public Color HairColor;
    }

    public struct ReceiveAuthenticateMessage : NetworkMessage
    {
        public string PlayFabId;
    }

    public struct ShutdownMessage : NetworkMessage { }

    [Serializable]
    public struct MaintenanceMessage : NetworkMessage
    {
        public DateTime ScheduledMaintenanceUTC;
    }

    public static class MaintenanceMessageFunctions
    {
        public static MaintenanceMessage Deserialize(this NetworkReader reader)
        {
            MaintenanceMessage msg = new MaintenanceMessage();

            var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
            msg.ScheduledMaintenanceUTC = json.DeserializeObject<DateTime>(reader.ReadString());

            return msg;
        }

        public static void Serialize(this NetworkWriter writer, MaintenanceMessage value)
        {
            var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
            var str = json.SerializeObject(value.ScheduledMaintenanceUTC);
            writer.Write(str);
        }
    }
}