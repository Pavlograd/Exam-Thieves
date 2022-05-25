using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[System.Serializable]
public struct ClassRoomElement
{
    public GameObject prefab;
    public int maxNumber; // Max number of this element. Don't put 0
    public float highness; // If object can be on top of something
}

public class GenerationObjects : NetworkBehaviour
{
    [SerializeField] GenerationObjectsData generationObjectsData;
    ClassRoomElement[] _classRoomElements; // List all possible objects in classroom
    [SerializeField] Transform objectsParent; // Every objects will have the same parent in hierarchy
    [SerializeField] NavMeshSurface surface;

    [SerializeField] private GameObject chest;
    [SerializeField] AlarmeGeneral alarmeManager;
    int size = 0;
    public Vector3[] codesLocations = new Vector3[3];

    public void GenerateObjects()
    {
        _classRoomElements = generationObjectsData._classRoomElements;
        size = GetComponent<ProceduralGeneration>().mapSize * GetComponent<ProceduralGeneration>().offsetSize / 2;

        SpawnObjects();

        surface.BuildNavMesh();

        if (isServer)
        {
            for (int i = 0; i < DifficultySettings.datas.nbrGuards; i++)
            {
                SpawnGuard(Random.Range(0, 2) == 0 ? generationObjectsData.mGuard : generationObjectsData.fGuard);
            }
            SpawnJanitor();
            SpawnTeachers();
        }

        SetIntercoms();
        alarmeManager.gameObject.SetActive(true);

        if (isServer)
        {
            GameObject chestGo = Instantiate(chest, GameObject.FindWithTag("Chest").transform.position, GameObject.FindWithTag("Chest").transform.rotation);
            NetworkServer.Spawn(chestGo);
        }

        GameObject chestGO = GameObject.Find("LockChest(Clone)");
        chestGO.GetComponent<AlarmeFinal>().alarmeGeneral = alarmeManager;

        // Locations are now uselles destroy them to optimize
        DestroyLocations();
    }

    void SpawnGuard(GameObject guard)
    {
        bool lookingForLocation = true;

        // SpawnGuardLocation is new SupervisorClassRoom
        Vector3 spawnLocation = GameObject.Find("SpawnGuardLocation").transform.position;

        while (lookingForLocation)
        {
            //Vector3 spawnTarget = spawnLocation;
            //float spawnJitter = size;

            //RANDOM
            //spawnTarget += new Vector3(Random.Range(-1.0f, 1.0f) * spawnJitter, 0, Random.Range(-1.0f, 1.0f) * spawnJitter);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnLocation, out hit, size, NavMesh.AllAreas) && (int)hit.position.y == 0)
            {
                //guard.transform.position = hit.position;
                GameObject guardGO = Instantiate(guard, hit.position, Quaternion.identity);
                guardGO.GetComponent<AICharacter>().size = size;
                //guard.SetActive(true);
                NetworkServer.Spawn(guardGO);
                lookingForLocation = false;
            }
        }
    }

    void SpawnJanitor()
    {
        int initSize = GetComponent<ProceduralGeneration>().mapSize;
        int offsetSize = GetComponent<ProceduralGeneration>().offsetSize;
        int initX = ((initSize + initSize / 10) / 2 + 1) * offsetSize;

        // * 2 is to prevent janitor going in the wall at the end or push player in the wall
        Vector3 startPosition = new Vector3(initX, 0.0f, offsetSize * 2);
        Vector3 endPosition = new Vector3(initX, 0.0f, offsetSize * initSize - (offsetSize * 2));

        NavMeshHit hit;
        NavMesh.SamplePosition(startPosition, out hit, 1, NavMesh.AllAreas);

        GameObject janitorGO = Instantiate(generationObjectsData.janitor, hit.position, Quaternion.identity);
        JanitorAI janitorAI = janitorGO.GetComponent<JanitorAI>();

        janitorAI.size = size;
        janitorAI.startPosition = hit.position;
        NavMesh.SamplePosition(endPosition, out hit, 1, NavMesh.AllAreas);

        janitorAI.endPosition = hit.position;

        NetworkServer.Spawn(janitorGO);
    }

    void SpawnTeachers()
    {
        Waypoints[] _waypoints = (Waypoints[])GameObject.FindObjectsOfType(typeof(Waypoints));

        for (int i = 0; i < _waypoints.Length; i++)
        {
            Waypoints item = _waypoints[i];

            if (Random.Range(0, 2) == 0)
            {
                item.Init(size);
                GameObject teacherGO = Instantiate(Random.Range(0, 2) == 0 ? generationObjectsData.teacher : generationObjectsData.fTeacher, item.GetNextWaypoint(null).position, Quaternion.identity);
                //WaypointMover wMover = teacherGO.GetComponent<WaypointMover>();
                AiProfesseur teacherAI = teacherGO.GetComponent<AiProfesseur>();
                teacherAI.idAlarm = i;
                //Alarme alarm = item.intercom.GetComponent<Alarme>();
                //alarm.aiProfesseur = teacherAI;
                //alarm.alarmeGeneral = alarmeManager;
                //alarm.professeur = teacherGO;
                teacherAI.enabled = true;
                teacherAI.Init(item);
                //wMover.Init(item);
                NetworkServer.Spawn(teacherGO);
                //item.intercom.SetActive(true);
            }
            else
            {
                // Destroy useless objects
                //Destroy(item.intercom);
                //Destroy(item);
            }
        }
    }

    void SetIntercoms()
    {
        AiProfesseur[] _teachers = (AiProfesseur[])GameObject.FindObjectsOfType(typeof(AiProfesseur));
        Waypoints[] _waypoints = (Waypoints[])GameObject.FindObjectsOfType(typeof(Waypoints));

        for (int i = 0; i < _teachers.Length; i++)
        {
            AiProfesseur teacherAI = _teachers[i];
            Waypoints item = _waypoints[teacherAI.idAlarm];
            Alarme alarm = item.intercom.GetComponent<Alarme>();

            alarm.aiProfesseur = teacherAI;
            alarm.alarmeGeneral = alarmeManager;
            alarm.professeur = teacherAI.gameObject;

            teacherAI.enabled = true;
            teacherAI.Init(item);

            item.intercom.SetActive(true);
        }
    }

    void SpawnObjects()
    {
        List<GameObject> locations = new List<GameObject>(GameObject.FindGameObjectsWithTag("ObjectLocation"));

        foreach (ClassRoomElement classRoomElement in _classRoomElements)
        {
            int number = 0;
            int locationChecked = 0;

            while (number != classRoomElement.maxNumber && locations.Count > 0) // In case maxNumber > to locations' size
            {
                if (locationChecked > locations.Count) break;
                int index = Random.Range(0, locations.Count);
                GameObject location = locations[Random.Range(0, locations.Count)];

                if (location.transform.position.y == classRoomElement.highness)
                {
                    // Remove the location to not spawn two objects at the same location
                    // Number++ no matter what to prevent crash
                    number++;
                    if (classRoomElement.prefab.name == "Code")
                    {
                        if (isServer)
                        {
                            if (!CodeIsAlreadyHere(location.transform.position))
                            {
                                //Debug.Log("Location Code: " + location.transform.position);
                                GameObject go = Instantiate(classRoomElement.prefab, location.transform.position, Quaternion.identity);
                                location.GetComponent<ObjectLocation>().PutObjectInStorage(go);
                                NetworkServer.Spawn(go);
                                locations.Remove(location);
                            }
                            else
                            {
                                number--;
                            }
                        }
                    }
                    else
                    {
                        GameObject go = Instantiate(classRoomElement.prefab, location.transform.position, Quaternion.identity, objectsParent);
                        location.GetComponent<ObjectLocation>().PutObjectInStorage(go);
                        locations.Remove(location);
                        //Debug.Log(location.transform.position);
                    }
                }
                locationChecked++;
            }
        }
    }

    bool CodeIsAlreadyHere(Vector3 pos)
    {
        for (int i = 0; i < codesLocations.Length; i++)
        {
            if (codesLocations[i] != Vector3.zero && Vector3.Distance(codesLocations[i], pos) < 15) return true;
        }

        for (int i = 0; i < codesLocations.Length; i++)
        {
            if (codesLocations[i] == Vector3.zero)
            {
                codesLocations[i] = pos;
                break;
            }
        }

        return false;
    }

    void DestroyLocations()
    {
        GameObject[] locations = GameObject.FindGameObjectsWithTag("ObjectLocation");

        foreach (GameObject location in locations)
        {
            Destroy(location);
        }
    }
}
