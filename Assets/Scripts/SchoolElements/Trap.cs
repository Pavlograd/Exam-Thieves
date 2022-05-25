using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("garde"))
        {
            // Stun gard or teacher
            AiAgent agent = other.gameObject.GetComponent<AiAgent>();

            agent.stateMachine.ChangeState(AiStateId.FallPaint);

            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Professeur"))
        {
            AiProfesseur professeur = other.gameObject.GetComponent<AiProfesseur>();

            professeur.professeurStateMachine.ChangeState(AiProfesseurStateId.FallPaint);

            Destroy(gameObject);
        }
    }
}
