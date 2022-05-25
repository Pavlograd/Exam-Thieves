using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiProfesseurStateMachine
{
    public AiProfesseurState[] professeurStates;
    public AiProfesseur professeur;
    public AiProfesseurStateId currentState;

    public AiProfesseurStateMachine(AiProfesseur professeur)
    {
        this.professeur = professeur;
        int numStates = System.Enum.GetNames(typeof(AiStateId)).Length;
        professeurStates = new AiProfesseurState[numStates];
    }

    public void RegisterState(AiProfesseurState state)
    {
        int index = (int)state.GetId();
        professeurStates[index] = state;
    }

    public AiProfesseurState GetState(AiProfesseurStateId professeurStateId)
    {
        int index = (int)professeurStateId;
        return professeurStates[index];
    }

    public void Update()
    {
        GetState(currentState)?.Update(professeur);
    }

    public void ChangeState(AiProfesseurStateId newState)
    {
        GetState(currentState)?.Exit(professeur);
        currentState = newState;
        GetState(currentState)?.Enter(professeur);
    }
}
