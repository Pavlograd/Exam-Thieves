using UnityEngine;

public class Locker : MonoBehaviour
{
    public Animator _animator;
    public Transform insidePosition;
    public Transform outsidePosition;
    public Quaternion rotation;
    public MeshRenderer _MeshLocker;
    public MeshRenderer _MeshDoor;
    public GameObject cadena;

   /* public Transform iconOuvert;
    public Transform iconFerme;

    private void Start()
    {
        iconOuvert = this.transform.Find("IconOuvert");
        
        iconFerme = this.transform.Find("IconFerme");
        
    }*/
    public void DestroyItself(Color closeColor)
    {
        rotation *= transform.parent.rotation;
        _MeshDoor.material.color = closeColor;
        _MeshLocker.material.color = closeColor;
        Destroy(_animator);
       // Destroy(iconOuvert.gameObject);
        Destroy(insidePosition.gameObject);
        Destroy(outsidePosition.gameObject);
        Destroy(this);
        
    }

    public void ActivateItself(Color openColor)
    {
        //FonctionIconeOuvert();
        rotation *= transform.parent.rotation;
        _MeshDoor.material.color = openColor;
        _MeshLocker.material.color = openColor;
        //Destroy(iconFerme.gameObject);
       // Destroy(iconFerme.gameObject);
        Destroy(cadena);
    }
   /* public void FonctionIconeOuvert()
    {
        iconOuvert = this.transform.Find("IconOuvert");
        iconOuvert.gameObject.SetActive(true);
        iconFerme = this.transform.Find("IconFerme");
        iconFerme.gameObject.SetActive(false);
    }*/
  
}
