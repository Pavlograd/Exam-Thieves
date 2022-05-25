using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLocation : MonoBehaviour
{
    [SerializeField] Storage2[] storages;

    public void PutObjectInStorage(GameObject go)
    {
        if (storages.Length != 0)
        {
            foreach (Storage2 item in storages)
            {
                item.goToSetActive = go;
            }
            go.SetActive(false);
        }
    }
}
