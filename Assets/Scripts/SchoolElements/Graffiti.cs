using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graffiti : MonoBehaviour
{
    [SerializeField] GraffitiData data;
    [SerializeField] SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer.sprite = data.graffitis[Random.Range(0, data.graffitis.Length)];
        _spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        // Need to change in script as it is a spriteRenderer

        var x = (transform.rotation.y == 0) ? ((Random.Range(0, 2) == 1) ? 1 : -1) : 0;
        var z = (transform.rotation.y == 0) ? 0 : ((Random.Range(0, 2) == 1) ? 1 : -1);

        transform.rotation = transform.rotation * Quaternion.Euler(0, (Random.Range(0, 2) == 1) ? 90f : -90f, 0);
        transform.position += new Vector3(1.17f * x, 1.0f, 1.17f * z);

        Destroy(this);
    }
}
