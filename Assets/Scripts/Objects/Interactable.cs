using UnityEngine;

public class Interactable : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    Color startColor;
    Color highColor;
    public string promptMessage;
    public Vector3 posPromptImage;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (meshRenderer != null)
        {
            startColor = meshRenderer.material.color;
            highColor = startColor + (0.2f * Color.white);
            meshRenderer.material.SetColor("_EmissionColor", Color.blue * 1f);
        }
    }

    public void UnHighlight()
    {
        if (meshRenderer != null)
        {
            Debug.Log("UnHighLight");
            meshRenderer.material.color = startColor;
            meshRenderer.material.DisableKeyword("_EMISSION");
        }
    }

    void HighLight()
    {
        if (meshRenderer != null)
        {
            Debug.Log("HighLight");
            meshRenderer.material.color = highColor;
            meshRenderer.material.EnableKeyword("_EMISSION");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerInteractableObject"))
        {
            PlayerController pController = other.gameObject.GetComponent<PlayerInteractableObject>().GetPlayerController();

            //if (pController.interactableObject != null) pController.interactableObject.SendMessage("UnHighlight");

            if (pController.interactableObject == null && meshRenderer != null)
            {
                pController.interactableObject = gameObject;
                HighLight();
            }
        }
        else if (other.gameObject.CompareTag("GuardInteractableObject"))
        {
            if (other.transform.parent.gameObject.GetComponent<AiAgent>().targeting.HasTarget)
            {
                GameObject guard = other.transform.parent.gameObject;
                this.SendMessage("GuardInspection", guard);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerInteractableObject"))
        {
            PlayerController pController = other.gameObject.GetComponent<PlayerInteractableObject>().GetPlayerController();

            if (pController.interactableObject == gameObject)
            {
                pController.interactableObject = null;
                UnHighlight();
            }
        }
    }
}
