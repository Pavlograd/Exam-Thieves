using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class GenerationNavMesh : MonoBehaviour
{
    [SerializeField] NavMeshSurface surface;
    [SerializeField] GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(prefab, Vector3.zero, Quaternion.identity);
        surface.BuildNavMesh();
    }
}
