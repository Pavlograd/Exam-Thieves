using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class credit : MonoBehaviour
{
    [SerializeField] private float startPosition;
    [SerializeField] private float endPosition;
    [SerializeField] private float speedMotion;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject SkipButton;
    private LTDescr id = null;

    public void StartMotionCrédit()
    {
        nextButton.SetActive(false);
        gameObject.transform.localPosition = new Vector3(0, startPosition, 0);
        id = LeanTween.moveLocal(gameObject, new Vector3(0, endPosition, 0), speedMotion).setOnComplete(() =>
        {
            nextButton.SetActive(true);
            id = null;
        });
        
    }

    public void Stop()
    {
        if (id != null)
            id.reset();
    }
}
