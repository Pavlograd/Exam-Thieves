using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VibrationDistanceZambonie : MonoBehaviour
{
    public GameObject zambonie = null;
    private PlayerCatched _playerCatched;

    // Start is called before the first frame update
    void Start()
    {
        zambonie = GameObject.Find("Janitor(Clone)");
        _playerCatched = gameObject.GetComponent<PlayerCatched>();
    }

    // Update is called once per frame
    void Update()
    {
        if (zambonie != null && !_playerCatched._catchedRun)
        {
            Debug.Log("Position Zamboni " + zambonie.transform.position);
            float distance = Vector3.Distance(transform.position, zambonie.transform.position);
            if (distance <= 5)
            {
                float valueVibr = 0.1f / distance;
                Gamepad.current.SetMotorSpeeds(valueVibr, valueVibr);
            }
            else
            {
                Gamepad.current.SetMotorSpeeds(0, 0);
            }
        }
        else if (zambonie == null)
        {
            zambonie = GameObject.FindWithTag("Zambonie");
        }
    }
}
