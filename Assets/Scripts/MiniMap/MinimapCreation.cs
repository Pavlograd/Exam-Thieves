using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapCreation : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Color colorDoor;
    [SerializeField] Color colorEmpty;
    [SerializeField] Color colorRoom;
    [SerializeField] Color colorCorridor;

    public void GenerateMap()
    {
        ProceduralGeneration pGeneration = GameObject.Find("ProcéduralGénération").GetComponent<ProceduralGeneration>();
        int[][] mapElements = pGeneration.GetMapElements();

        // Set Size for update
        GetComponent<MinimapUpdate>().Init(mapElements[0].Length);

        Texture2D texture = new Texture2D(mapElements[0].Length, mapElements[0].Length);
        Color[] pixels = new Color[mapElements[0].Length * mapElements[0].Length];

        // Prevent blurry image
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        int indexPixel = 0;

        for (int i = 0; i < mapElements[0].Length; i++)
        {
            for (int y = 0; y < mapElements[0].Length; y++)
            {
                switch (mapElements[i][y])
                {
                    case (int)GenerationElement.Door:
                        pixels[indexPixel] = colorDoor;
                        break;
                    case (int)GenerationElement.Corridor:
                        pixels[indexPixel] = colorCorridor;
                        break;
                    case (int)GenerationElement.Room:
                        pixels[indexPixel] = colorRoom;
                        break;
                    default:
                        // Empty
                        pixels[indexPixel] = colorEmpty;
                        break;
                }
                indexPixel++;
            }
        }

        texture.SetPixels(pixels);
        texture.name = "MiniMap";
        texture.Apply();

        image.sprite = Sprite.Create(texture, new Rect(0, 0, mapElements[0].Length, mapElements[0].Length), new Vector2(0.5f, 0.5f));
        image.sprite.name = "MiniMap";
        image.material.mainTexture = texture;

        // Script is now useless
        Destroy(this);
    }
}
