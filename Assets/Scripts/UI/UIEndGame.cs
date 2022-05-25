using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEndGame : MonoBehaviour
{
    [SerializeField] GameObject victory;
    [SerializeField] GameObject defeat;

    public void OnAButton()
    {
        Debug.Log("Press A");

        GameManager.instance.EndGame();
    }

    public void SetVictory()
    {
        victory.SetActive(true);
    }

    public void SetDefeat()
    {
        defeat.SetActive(true);
    }
}
