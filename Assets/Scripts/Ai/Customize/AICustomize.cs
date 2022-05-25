using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICustomize : MonoBehaviour
{
    [SerializeField] private GameObject HairGO;
    [SerializeField] private GameObject[] SkinGO;

    [SerializeField] private CustomizeListColor hairColors;
    [SerializeField] private CustomizeListColor skinColors;

    // Start is called before the first frame update
    void Start()
    {
        if (HairGO != null) HairGO.GetComponent<MeshRenderer>().material = hairColors.colors[Random.Range(0, hairColors.colors.Length)];

        Material material = skinColors.colors[Random.Range(0, skinColors.colors.Length)];

        foreach (GameObject item in SkinGO) item.GetComponent<SkinnedMeshRenderer>().material = material;

        Destroy(this);
    }
}
