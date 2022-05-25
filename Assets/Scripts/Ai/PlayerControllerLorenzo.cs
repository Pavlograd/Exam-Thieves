using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerLorenzo : MonoBehaviour
{
    private CharacterController cc;
    // Variables pour le deplacement
    public float moveSpeed;
    public float jumpForce;
    public float gravity;
    // Vecteur direction
    private Vector3 moveDir;
    public GameObject flash;
    
    bool isWalking = false;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        flash.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        // Calcul de la direction
        moveDir = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, moveDir.y, Input.GetAxis("Vertical") * moveSpeed);
        
        /*
        
        // Check de la touche espace
        if(Input.GetButtonDown("Jump")&& cc.isGrounded)
        {
            // On saute
            moveDir.y = jumpForce;
            
                
        }
        // On applique la gravite
        moveDir.y -= gravity * Time.deltaTime;*/
        
        // Si deplacement
        if (moveDir.x != 0 || moveDir.z != 0)
        {
            isWalking = true; // le perso marche
            // On tourne le personnage
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(moveDir.x, 0, moveDir.z)), 0.15f );
        }
        else
        {
            isWalking = false; // le perso ne marche pas
        }
        if (Input.GetMouseButtonDown(0)) 
        {
            flash.SetActive(true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            flash.SetActive(false) ;
        }





        // On applique le deplacement
        cc.Move(moveDir * Time.deltaTime);
    }




}
