using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnimationPressAnayKEy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.LeanScale(new Vector3(1, 1), 5f).setLoopPingPong();
    }
}
