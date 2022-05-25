using UnityEngine;


[CreateAssetMenu(fileName = "DifficultyData", menuName = "DifficultyData/DifficultyData")]
public class DifficultyData : ScriptableObject
{
    public int mapSize = 35;
    public float percentageLockers = 10;
    public int nbrGuards = 4;
    public int FOVGuards = 15;
    public float speedGuardsMult = 1;
}
