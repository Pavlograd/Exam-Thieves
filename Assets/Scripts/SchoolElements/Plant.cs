using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] GameObject[] plants;
    void Start()
    {
        int index = Random.Range(0, plants.Length);

        for (int i = 0; i < plants.Length; i++)
        {
            if (i == index)
            {
                plants[i].transform.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            }
            else
            {
                Destroy(plants[i]);
            }
        }

        Destroy(this);
    }
}
