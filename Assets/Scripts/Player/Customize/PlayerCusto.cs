using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCusto : MonoBehaviour
{
    [SerializeField] private GameObject HairGO;
    [SerializeField] private GameObject[] SkinGO;
    [SerializeField] private CustomizeListColor hairColorList;
    [SerializeField] private CustomizeListColor skinColorList;
    [SerializeField] private BasicColorPlayer _basicColorPlayer;
    private BasicColorPlayer actualBasicColorPlayer;

    private void Awake()
    {
        Reset();
    }

    public BasicColorPlayer GetSkinBasicColorPlayerColor()
    {
        return actualBasicColorPlayer;
    }

    public void Reset()
    {
        Debug.Log("Call Reset");
        ChangeColor(_basicColorPlayer.skinColor, _basicColorPlayer.hairColor);
    }

    public void ChangeColor(int skinColor, int hairColor)
    {
        Debug.Log(skinColor + " hair " + hairColor);
        actualBasicColorPlayer.hairColor = hairColor;
        actualBasicColorPlayer.skinColor = skinColor;
        foreach (GameObject skin in SkinGO)
        {
            if (skin.CompareTag("JambeArtiste"))
            {
                Material[] matArray = skin.GetComponent<SkinnedMeshRenderer>().materials;
                matArray[1] = skinColorList.colors[skinColor];
                skin.GetComponent<SkinnedMeshRenderer>().materials = matArray;
            }
            else
                skin.GetComponent<SkinnedMeshRenderer>().material = skinColorList.colors[skinColor];
        }
        HairGO.GetComponent<MeshRenderer>().material = hairColorList.colors[hairColor];
    }
}
