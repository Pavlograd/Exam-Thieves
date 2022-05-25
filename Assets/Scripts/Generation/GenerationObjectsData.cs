using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationData", menuName = "GenerationData/GenerationObjects")]
public class GenerationObjectsData : ScriptableObject
{
    public ClassRoomElement[] _classRoomElements; // List all possible objects in classroom
    public GameObject mGuard;
    public GameObject fGuard;
    public int nbrGuards;
    public GameObject janitor;
    public GameObject teacher;
    public GameObject fTeacher;
}
