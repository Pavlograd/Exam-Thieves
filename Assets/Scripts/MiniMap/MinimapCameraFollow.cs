using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [SerializeField] private MiniMapSettings miniMapSettings;
    [SerializeField] private float cameraHeight;

    // Start is called before the first frame update
    private void Awake()
    {
        miniMapSettings = GetComponentInParent<MiniMapSettings>();
        cameraHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = miniMapSettings.targetToFollow.transform.position;

        transform.position = new Vector3(targetPosition.x, targetPosition.y + cameraHeight, targetPosition.z);

        if(miniMapSettings.rotateWithTheTarget)
        {
            Quaternion targetRotation = miniMapSettings.targetToFollow.transform.rotation;

            transform.rotation = Quaternion.Euler(90, targetRotation.eulerAngles.y, 0);
        }    
    }
}
