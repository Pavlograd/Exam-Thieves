using UnityEngine;


[CreateAssetMenu(fileName = "GenerationData", menuName = "GenerationData/ProceduralGeneration")]
public class ProceduralGenerationData : ScriptableObject
{
    public Room[] rooms; // List all possible rooms
    public SchoolElement[] schoolElements; // List all possible school elements
    public GameObject[] prefabCorridor; // Will be a list later for intersections
    public int offsetSize = 3; // Offset in unit between corridors
    public int mapSize = 50; // MapSize is exp be careful 50 is small and 100 is large
}
