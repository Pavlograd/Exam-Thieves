using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEndGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.LeanScale(new Vector3(1, 1), 2f);
        transform.LeanRotate(new Vector3(0.0f, 0.0f, 330.0f), 3f);
    }
}
