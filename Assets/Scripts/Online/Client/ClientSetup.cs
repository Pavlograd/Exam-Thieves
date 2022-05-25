using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using PlayFab.Networking;
using UnityEngine.UI;
using Mirror;

public class ClientSetup : MonoBehaviour
{
    [SerializeField] private Text _text;
    [HideInInspector] public string EntityID;
    [HideInInspector] public string playfabID;
    [SerializeField] private string BuildId;

    // Start is called before the first frame update
    void Start()
    {
        NetworkClient.RegisterHandler<ShutdownMessage>(OnServerShutdown);
        NetworkClient.RegisterHandler<MaintenanceMessage>(OnMaintenanceMessage);
        authentification();
    }

    private void authentification()
    {
        var request = new LoginWithCustomIDRequest
        {
            TitleId = PlayFabSettings.TitleId,
            CustomId = Guid.NewGuid().ToString(),
            CreateAccount = true

        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnMaintenanceMessage(MaintenanceMessage msg)
    {
        var message = msg;
        //_text.text = string.Format("Maintenance is scheduled for: {0}", message.ScheduledMaintenanceUTC.ToString("MM-DD-YYYY hh:mm:ss"));
    }

    private void OnServerShutdown(ShutdownMessage msg)
    {
        //_text.text = "Shutdown In Progress (You can play alone)";
        //_text.color = Color.red;
        NetworkClient.Disconnect();
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!" + result.PlayFabId);
        EntityID = result.EntityToken.Entity.Id;
        //_text.text = EntityID.ToString();
        playfabID = result.PlayFabId;
        //UpdateDisplayName(); On utilisera cela quand on aura le système de username
        ResquestMultiplayerServer(); //Not request because we have matchmaking
    }

    private void UpdateDisplayName()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = "Zepoulpe"
        }, result =>
        {
            Debug.Log("The player's display name is now: " + result.DisplayName);
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void ResquestMultiplayerServer()
    {
        Debug.Log("[CLientStartup] Requeste a Server");

        RequestMultiplayerServerRequest request = new RequestMultiplayerServerRequest
        {
            BuildId = BuildId,
            SessionId = Guid.NewGuid().ToString("N"),
            PreferredRegions = new List<string> { "EastUS" }
        };

        PlayFabMultiplayerAPI.RequestMultiplayerServer(request, OnResquestMultiplayerServer, OnResquestMultiplayerServerError);
    }

    private void OnResquestMultiplayerServer(RequestMultiplayerServerResponse response)
    {
        if (response == null) return;

        Debug.Log("IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);

        UnityNetworkServer.Instance.networkAddress = response.IPV4Address;
        UnityNetworkServer.Instance.GetComponent<kcp2k.KcpTransport>().Port = (ushort)response.Ports[0].Num;

        UnityNetworkServer.Instance.StartClient();

        NetworkClient.connection.Send<ReceiveAuthenticateMessage>(new ReceiveAuthenticateMessage()
        {
            PlayFabId = playfabID
        });
    }

    private void OnResquestMultiplayerServerError(PlayFabError error)
    {
        Debug.Log(error);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

}
