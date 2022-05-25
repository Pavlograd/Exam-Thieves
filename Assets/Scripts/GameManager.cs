using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using PlayFab.Networking;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject uiEndGame;
    [SerializeField] private DifficultyData[] listDiff;
    [SerializeField] private float timerInMinute;
    public int numberMaxCode;
    public int numberMaxLife;

    public float secondToEscape = 10f;
    public int nbPressToEscape = 20; // 20 - 33 - 40 difficulty ?

    private const string playerIdPrefix = "Player";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static GameManager instance;

    [SyncVar]
    public bool endGame = false;

    [SyncVar(hook = nameof(HandleDiff))]
    public int _difficulty;

    [SyncVar]
    public int numberOfCode = 0;

    [SyncVar]
    public int numberOfLife = 0;

    [SyncVar]
    public int numberCatch = 0;

    [SyncVar]
    public float timeValue = 0;

    private Coroutine timerCoroutine;

    public void StartTimer()
    {
        timeValue = timerInMinute * 60;
        if (isServer)
            timerCoroutine = StartCoroutine(TimerCouroutine());
    }

    IEnumerator TimerCouroutine()
    {
        while (timeValue > 0)
        {
            yield return new WaitForSeconds(1);
            timeValue--;
        }
        GameManager.instance.endGame = true;
        Defeat();
        yield break;
    }

    public void HandleDiff(int oldValue, int newValue) => SetDiffValue();

    private void SetDiffValue()
    {
        DifficultySettings.datas = listDiff[_difficulty];
        Debug.LogWarning("Nombre de Garde" + DifficultySettings.datas.nbrGuards);
    }

    public static bool AllDeath()
    {
        return GetAllPlayer().All(variabPlayer => variabPlayer.isDeath);
    }
    

    public void Quit() => UnityNetworkServer.Instance.StopClient();

    public float GetNbPressToEscapeDivide()
    {
        return nbPressToEscape + 8 * numberCatch; // 8 -10 -12 difficulty ?
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        Debug.LogError("Plus d'une instance de GameManager dans la scène");
    }

    private void UpdateCode(int oldValue, int newValue)
    {

    }

    public void SetMainCameraActive(bool isActive)
    {
        if (mainCamera)
        {
            mainCamera.SetActive(false);
            //mainCamera.GetComponent<Camera>().enabled = !mainCamera.GetComponent<Camera>().enabled;
        }
    }

    public static void RegisterPlayer(string netId, Player player)
    {
        string playerId = playerIdPrefix + netId;
        Debug.Log(playerId);
        players.Add(playerId, player);
        player.transform.name = playerId;
    }
    public static void UnRegisterPlayer(string playerId)
    {
        players.Remove(playerId);
    }

    public static Player GetPlayer(string playerId)
    {
        return players[playerId];
    }

    public static Player[] GetAllPlayer()
    {
        return players.Values.ToArray();
    }

    public void StopTimer()
    {
        if (isServer)
            StopCoroutine(timerCoroutine);
    }

    [ClientRpc]
    public void Victory()
    {
        Debug.Log("Victory");

        DestroyEntities();

        GameObject go = Instantiate(uiEndGame, Vector3.zero, Quaternion.identity);

        go.GetComponent<UIEndGame>().SetVictory();
        GameObject.Find("Music").GetComponent<MusicManager>().Win();
    }

    [ClientRpc]
    public void Defeat()
    {
        Debug.Log("Defeat");

        DestroyEntities();

        GameObject go = Instantiate(uiEndGame, Vector3.zero, Quaternion.identity);

        go.GetComponent<UIEndGame>().SetDefeat();
        GameObject.Find("Music").GetComponent<MusicManager>().Loose();
    }

    void DestroyEntities()
    {
        GetAllPlayer().All(variabPlayer => variabPlayer.gameObject.GetComponent<PlayerController>().canMove = false);

        GameObject[] guards = GameObject.FindGameObjectsWithTag("garde");
        GameObject[] profs = GameObject.FindGameObjectsWithTag("Professeur");

        foreach (GameObject item in guards)
        {
            NetworkServer.Destroy(item);
        }

        foreach (GameObject item in profs)
        {
            NetworkServer.Destroy(item);
        }
    }

    public void EndGame()
    {
        Debug.Log("End the game now");
        Quit();
    }
}
