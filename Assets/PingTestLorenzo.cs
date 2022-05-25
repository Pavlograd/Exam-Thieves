using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingTestLorenzo : MonoBehaviour
{
    [SerializeField] GameObject ping;
    [SerializeField] GameObject previousPing;
  
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (previousPing == null)
            {
                previousPing = Instantiate(ping, transform.position, ping.transform.rotation);
            }
            else
            {
                Destroy(previousPing);
                previousPing = Instantiate(ping, transform.position, ping.transform.rotation);
            }

        }
    }
}
