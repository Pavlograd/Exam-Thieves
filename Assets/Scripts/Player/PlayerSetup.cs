using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using PlayFab.Networking;
using StaticClassSettingsGame;

public class PlayerSetup : NetworkBehaviour
{
    [SyncVar] public string type;
    [SyncVar] public int skinColor;
    [SyncVar] public int hairColor;
    public PlayerGFX playerGfx;

    [SerializeField] private Behaviour[] compenentsToDisable;
    [SerializeField] private GameObject cicleInfo;

    [SerializeField] private string dontDrawLayerName = "DontDraw";
    // Start is called before the first frame update

    [SerializeField] private GameObject playerUIPrefabs;

    public Vector3 startPosition;

    public GameObject playerUIInstance;


    void Start()
    {
        playerGfx = gameObject.GetComponent<PlayerGFX>();
        playerGfx.runPlayerGFX(type, skinColor, hairColor);

        Debug.Log("Start player");
        if (!isLocalPlayer)
        {
            Debug.Log("Won't activate generation");
            DisableComponents();
        }
        else
        {
            Debug.Log("Activate procedural generation");
            GameObject procedural = GameObject.FindWithTag("Procedural");
            procedural.GetComponent<ProceduralGeneration>().enabled = true;
            gameObject.GetComponent<PlayerController>().enabled = true;
            GameManager.instance.SetMainCameraActive(false);
            playerUIInstance = Instantiate(playerUIPrefabs);
            PlayerUI playerUIScript = playerUIInstance.GetComponent<PlayerUI>();
            playerUIScript.SetPlayerController(gameObject.GetComponent<PlayerController>());
            playerUIScript.SetPowerImage(playerGfx.currentPlayerData.powerImage);
            ActiveLayer(cicleInfo);
        }
    }

    public void TpPlayer()
    {
        startPosition = GameObject.FindWithTag("Spawn").transform.position;
        gameObject.transform.position = new Vector3(startPosition.x, 0, startPosition.z);
        gameObject.transform.rotation = GameObject.FindWithTag("Spawn").transform.rotation;
    }

    [Server]
    public void SetType(string _type, int skin, int hair)
    {
        Debug.Log("Rpc");
        type = _type;
        skinColor = skin;
        hairColor = hair;
    }

    public void ActiveLayer(GameObject obj)
    {
        Debug.Log(obj.name);
        if (isLocalPlayer)
            SetLayerRecursively(obj, LayerMask.NameToLayer(dontDrawLayerName));
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    /* [Command]
     void CmdSetUsername(string userID, string username)
     {
         Player player = GameManager.GetPlayer(userID);
         if (player != null)
         {
             Debug.Log(username + " join");
             player.username = username;
         }
     }*/


    //Methode qui se lance quand un joueur rejoint le serveur
    public override void OnStartClient()
    {
        base.OnStartClient();

        RegisterPlayer();

    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        //Activer si build server
        //RegisterPlayer();
    }

    private void RegisterPlayer()
    {
        string netId = gameObject.GetComponent<NetworkIdentity>().netId.ToString();
        Player player = gameObject.GetComponent<Player>();
        GameManager.RegisterPlayer(netId, player);
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);
        if (isLocalPlayer)
            GameManager.instance.SetMainCameraActive(true);
        GameManager.UnRegisterPlayer(transform.name);
    }

    private void DisableComponents()
    {
        for (int i = 0; i != compenentsToDisable.Length; i++)
        {
            compenentsToDisable[i].enabled = false;
        }
    }
}
