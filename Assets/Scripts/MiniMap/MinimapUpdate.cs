using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUpdate : MonoBehaviour
{
    [SerializeField] Image imagePlayer1;
    [SerializeField] Image imagePlayer2;
    [SerializeField] Image imageFog;
    [SerializeField] Image imagepingPlayer1;
    [SerializeField] Image imagepingPlayer2;

    [SerializeField] GameObject ping;
    [SerializeField] GameObject previousPing1;
    [SerializeField] GameObject previousPing2;

    public Transform player1transform;
    public Transform player2transform;
    public Transform pingPlayer1transform;
    public Transform pingPlayer2transform;

    Vector3 oldPosition1;
    Vector3 oldPosition2;
    public float size;
    public float imageSize = 220;
    float ratio;
    float halfSize;
    int[][] mapFog;
    Texture2D texture;

    public void Init(int _size)
    {
        size = _size * 3;
        ratio = imageSize / size;
        halfSize = imageSize / 2;

        texture = new Texture2D(_size, _size);
        Color[] pixels = new Color[_size * _size];

        // Prevent blurry image
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < _size * _size; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.name = "MiniMapFog";
        texture.Apply();

        imageFog.sprite = Sprite.Create(texture, new Rect(0, 0, _size, _size), new Vector2(0.5f, 0.5f));
        imageFog.sprite.name = "MiniMapFog";
        imageFog.material.mainTexture = texture;

        //Animationn scale pings
        imagepingPlayer1.transform.LeanScale(new Vector3(1, 1), 2f).setLoopPingPong();
        imagepingPlayer2.transform.LeanScale(new Vector3(1, 1), 2f).setLoopPingPong();


        // MiniMap set to 20fps
        InvokeRepeating("UpdatePlayers", 1.0f, 0.05f);
    }

    // Update is called once per frame


    void UpdatePlayers()
    {
        if (player1transform == null || player2transform == null)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length == 2)
            {
                player1transform = GameObject.FindGameObjectsWithTag("Player")[0].transform;
                player2transform = GameObject.FindGameObjectsWithTag("Player")[1].transform;

                /*if (player2transform.GetComponent<PlayerSetup>().isLocalPlayer)
                {
                    player2transform = GameObject.FindGameObjectsWithTag("Player")[0].transform;
                    player1transform = GameObject.FindGameObjectsWithTag("Player")[1].transform;
                }*/

                player1transform.gameObject.GetComponent<PlayerController>().minimap = this;
                player2transform.gameObject.GetComponent<PlayerController>().minimap = this;
            }
        }
        else
        {
            UpdatePlayer(player1transform, imagePlayer1.rectTransform, oldPosition1);
            UpdatePlayer(player2transform, imagePlayer2.rectTransform, oldPosition2);
        }
    }


    void UpdatePlayer(Transform transform, RectTransform rTransform, Vector3 oldPosition)
    {
        Vector3 position = transform.position;

        // Prevent updating while player didn't moved
        if (position == oldPosition) return;

        oldPosition1 = position;

        // Y and Y are reversed in UI
        rTransform.localPosition = new Vector3(position.x * ratio - halfSize, position.z * ratio - halfSize, 0);
        rTransform.localRotation = Quaternion.Euler(0f, 0f, transform.eulerAngles.y * -1);

        // Clear fog in a zone
        Vector2Int position2D = new Vector2Int((int)position.x / 3, (int)position.z / 3);
        int zoneSize = 2;

        for (int i = position2D.y - zoneSize; i < position2D.y + zoneSize; i++)
        {
            for (int y = position2D.x - zoneSize; y < position2D.x + zoneSize; y++)
            {
                texture.SetPixel(y, i, Color.clear);
            }
        }
        texture.Apply();
    }

    public void CreatePing(PlayerController pController)
    {
        Debug.Log("CreatePing");
        Image pingImage = pController.transform == player1transform ? imagepingPlayer1 : imagepingPlayer2;
        Vector3 position = pController.transform == player1transform ? player1transform.position : player2transform.position;
        pingImage.gameObject.SetActive(true);
        pingImage.transform.localPosition = new Vector3(position.x * ratio - halfSize, position.z * ratio - halfSize, 0);

        if (pController.transform == player1transform)
        {
            if (previousPing1 == null)
            {
                previousPing1 = Instantiate(ping, player1transform.position, ping.transform.rotation);
            }
            else
            {
                Destroy(previousPing1);
                previousPing1 = Instantiate(ping, player1transform.position, ping.transform.rotation);
            }
        }
        else
        {
            if (previousPing2 == null)
            {
                previousPing2 = Instantiate(ping, player2transform.position, ping.transform.rotation);
            }
            else
            {
                Destroy(previousPing2);
                previousPing2 = Instantiate(ping, player2transform.position, ping.transform.rotation);
            }
        }

    }
}
