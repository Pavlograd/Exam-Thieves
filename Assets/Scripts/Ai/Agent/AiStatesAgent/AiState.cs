using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// video: https://www.youtube.com/watch?v=1H9jrKyWKs0&list=PLyBYG1JGBcd009lc1ZfX9ZN5oVUW7AFVy&index=3
public enum AiStateId
{
    FindTarget,
    ChasePlayer,
    Alarm,
    Stun,
    FallPaint,
    Capture,
}
public interface AiState
{
    AiStateId GetId();
    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
    
}

