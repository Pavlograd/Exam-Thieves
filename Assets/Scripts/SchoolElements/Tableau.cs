using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tableau : MonoBehaviour
{
    [SerializeField] MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0, (Random.Range(0, 2) == 1) ? 0f : 180f, 0);

        Destroy(this);
    }
}
