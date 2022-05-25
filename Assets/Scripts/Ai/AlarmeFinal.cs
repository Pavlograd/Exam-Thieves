using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmeFinal : MonoBehaviour
{
    public AlarmeGeneral alarmeGeneral;
    // Start is called before the first frame update
    void Start()
    {
        //alarmeGeneral = GameObject.Find("AlarmeManager").GetComponent<AlarmeGeneral>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.numberOfCode == GameManager.instance.numberMaxCode)
        {
            alarmeGeneral.target = this.gameObject;
            alarmeGeneral._alarmeFinal = true;

            Destroy(this);
        }
    }
}
