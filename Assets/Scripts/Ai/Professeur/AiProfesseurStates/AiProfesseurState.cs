using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AiProfesseurStateId
{
    Patrol,
    WantsToAlarm,
    Alarm,
    FallPaint,
    Stun,
}
public interface AiProfesseurState 
{
    AiProfesseurStateId GetId();
    void Enter(AiProfesseur professeur);
    void Update(AiProfesseur professeur);
    void Exit(AiProfesseur professeur);

}
