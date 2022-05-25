using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingSystem : MonoBehaviour
{
    private MinimapUpdate minimapUpdate;
    // Start is called before the first frame update
    void Start()
    {
        minimapUpdate = GetComponent<MinimapUpdate>();
        StartCoroutine(ArretPing());
    }

    // Update is called once per frame
  IEnumerator ArretPing()
    {
        yield return new WaitForSeconds(30);
        Destroy(gameObject);
        
    }
    private void OnDestroy()
    {
        Debug.Log("byebye");
    }
}
