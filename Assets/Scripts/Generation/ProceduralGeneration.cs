using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Probability : int // all probability are 1 / X with X exclusive
{
    twoCorridor = 4,
    direction = 3
}

enum Gap : int // Possible Gap for an element from [Element - X to Element + X]
{
    corridor = 5,
}

[System.Serializable]
public enum Corridor // Later for prefabs
{
    Straight,
    DeadEnd,
    Corner,
    ThreeIntersections,
    Star
}

enum GenerationElement : int
{
    Empty = 0,
    Corridor = 1,
    Room = 2,
    Door = 3,
}

[System.Serializable]
public struct Room
{
    public GameObject prefab;
    public int width; // X size
    public int heigth; // Y size
    public int rotation; // Default is always 0 is it used for diversity later in the code
    public float probability; // Probability make a rare room elss likely to be at the same place om everymap like top left or top right
    public int maxNumber; // Max number of this room. if 0 then unlimited
    public bool dynamicMaxNumber; // Max numebr will also depend of size of the map
    public Vector2Int corridorPosition; // Corridor position when rotation is 0
    public Vector2Int doorPosition; // Door position when rotation is 0
}

[System.Serializable]
public struct SchoolElement
{
    public GameObject prefab;
    public Corridor[] corridors; // List corridor the element can spawn on
    public float probability; // Probability make a rare element elss likely to be at the same place om everymap like top left or top right
    public int maxNumber; // Max number of this element. if 0 then unlimited
    public int number; // number of this element
    public bool dynamicMaxNumber; // Max numebr will also depend of size of the map
}

public class ProceduralGeneration : NetworkBehaviour
{
    [SerializeField] ProceduralGenerationData generationData;
    Room[] rooms; // List all possible rooms
    SchoolElement[] schoolElements; // List all possible school elements
    GameObject[] prefabCorridor; // Will be a list later for intersections
    [SerializeField] Transform corridorsParent; // Just to put all corridors in the same parent
    [SerializeField] Transform roomsParent; // Just to put all rooms in the same parent
    [SerializeField] Transform schoolElementsParent; // Just to put all schoolElements in the same parent
    public int offsetSize = 3; // Offset in unit between corridors
    private GenerationObjects _generationObjects; // Script to generate Objects in ClassRooms
    public int mapSize = 50; // MapSize is exp be careful 50 is small and 100 is large
    [SyncVar(hook = "GetSeed")]
    public int seed = 0; // MapSize is exp be careful 50 is small and 100 is large
    int mapMaxSize = 56; // MaxSize will always be mapSize + mapSize / 10 + 1
    int[][] mapElements;
    int numberRoom = 0;

    // Value to prevent supervisorRoom next To StartRoom
    Vector3 startRoomPosition = Vector3.zero;

    void GetSeed(int hold, int _seed)
    {
        if (isServer) return;
        seed = _seed;
    }

    public int[][] GetMapElements()
    {
        return mapElements;
    }

    void InitValues()
    {
        rooms = generationData.rooms;
        schoolElements = generationData.schoolElements;
        prefabCorridor = generationData.prefabCorridor;
        offsetSize = generationData.offsetSize;
        mapSize = DifficultySettings.datas.mapSize;
        _generationObjects = gameObject.GetComponent<GenerationObjects>();

        GameObject.Find("Music").GetComponent<MusicManager>().SwitchToGameMusic();
    }

    void Start()
    {
        InitValues();

        if (isServer)
        {
            seed = Random.Range(1, 100);
        }
        Vector2 direction = Vector2.up;
        mapMaxSize = mapSize + mapSize / 10;
        Vector2 origin = new Vector2(mapMaxSize / 2 + 1, 0.0f);

        // Init random seed
        Random.InitState(seed);

        InitDoubleArray(mapMaxSize);

        // Create main corridor
        CreateCorridors(mapSize, origin, direction);

        for (int i = 0; i < rooms.Length; i++)
        {
            Room room = rooms[i];

            CreateRooms(room, 0);

            //Debug.Log(room.prefab.name + " " + numberRoom);

            if (room.maxNumber != 0 && numberRoom != room.maxNumber)
            {
                room.probability = 100;
                // Keep this Debug it's important for randomization check-up
                //Debug.Log("Not space found: " + room.prefab.name);
                CreateRooms(room, numberRoom);
            }
        }

        CreateCorridorsFromMap();

        // Now move camera high enough
        // For tests only
        MoveCameraToCenter();
        GameObject minimapGO = GameObject.FindWithTag("MiniMap").transform.GetChild(0).gameObject;
        minimapGO.SetActive(true);
        MinimapCreation minimap = minimapGO.GetComponent<MinimapCreation>();

        minimap.GenerateMap();

        _generationObjects.GenerateObjects();

        // Script are now useless
        //Destroy(_generationObjects);
        Destroy(this);

        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject _gameObject in player)
        {
            _gameObject.GetComponent<PlayerSetup>().TpPlayer();
        }

    }

    void CreateRooms(Room room, int currentNumberRoom)
    {
        int limit = room.heigth > room.width ? room.heigth : room.width; // Limit needs to ne the maximum length
        numberRoom = currentNumberRoom;

        for (int y = limit; y < mapElements[0].Length - limit; y++)
        {
            for (int x = limit; x < mapElements[0].Length - limit; x++)
            {
                if (mapElements[y][x] == (int)GenerationElement.Empty)
                {
                    RoomCreationFromnPosition(new Vector2Int(x, y), room);
                }

                if (room.maxNumber != 0 && numberRoom >= room.maxNumber) return;
            }
        }
    }

    void RoomCreationFromnPosition(Vector2Int position, Room room)
    {
        if (CanCreateRoomFromPosition(position, room)) return;
        if (CanCreateRoomFromPosition(position, InverseOfRoom(room))) return;
        if (CanCreateRoomFromPosition(position, RotateRoom(room))) return;
        if (CanCreateRoomFromPosition(position, InverseOfRoom(RotateRoom(room)))) return;
    }

    bool CanCreateRoomFromPosition(Vector2Int position, Room room)
    {
        if (CanCreateARoom(position, room) && RoomHasCorridor(position, room))
        {
            CreateARoom(position, room);
            return true;
        }
        return false;
    }

    Room InverseOfRoom(Room room)
    {
        Room newRoom = room;

        newRoom.rotation -= 180;
        newRoom.corridorPosition = new Vector2Int(-1 * room.corridorPosition.x, -1 * room.corridorPosition.y);
        newRoom.doorPosition = new Vector2Int(-1 * room.doorPosition.x, -1 * room.doorPosition.y);

        return newRoom;
    }

    Room RotateRoom(Room room)
    {
        Room newRoom = room;

        newRoom.heigth = room.width;
        newRoom.width = room.heigth;
        newRoom.rotation = 90;
        newRoom.corridorPosition = new Vector2Int(room.corridorPosition.y, -1 * room.corridorPosition.x);
        newRoom.doorPosition = new Vector2Int(room.doorPosition.y, -1 * room.doorPosition.x);

        return newRoom;
    }

    bool CanCreateARoom(Vector2Int position, Room room)
    {
        for (int y = position.y - (room.heigth - 1); y <= position.y + (room.heigth - 1); y++)
        {
            for (int x = position.x - (room.width - 1); x <= position.x + (room.width - 1); x++)
            {
                if (mapElements[y][x] != (int)GenerationElement.Empty)
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool RoomHasCorridor(Vector2Int position, Room room)
    {
        if (mapElements[position.y + room.corridorPosition.y][position.x + room.corridorPosition.x] == (int)GenerationElement.Corridor) return true;

        return false;
    }

    void CreateARoom(Vector2Int position, Room room)
    {
        float probability = Random.Range(0.0f, 100.0f);

        if (probability >= room.probability) return;

        if (room.prefab.name == "StartRoom")
        {
            if (position.y < 10) return;
            startRoomPosition = new Vector3(position.x * offsetSize, 0.0f, position.y * offsetSize);
        }
        else if (room.prefab.name == "SupervisorRoom" && Vector3.Distance(new Vector3(position.x * offsetSize, 0.0f, position.y * offsetSize), startRoomPosition) < 30)
        {
            // Near starRoom
            return;
        }

        numberRoom++;

        for (int y = position.y - (room.heigth - 1); y <= position.y + (room.heigth - 1); y++)
        {
            for (int x = position.x - (room.width - 1); x <= position.x + (room.width - 1); x++)
            {
                // Prevent new Rooms on eachothers
                mapElements[y][x] = (int)GenerationElement.Room;
            }
        }

        mapElements[position.y + room.doorPosition.y][position.x + room.doorPosition.x] = (int)GenerationElement.Door; // To connect with corridor
        GameObject roomGO = Instantiate(room.prefab, new Vector3(position.x * offsetSize, 0.0f, position.y * offsetSize), Quaternion.Euler(0.0f, room.rotation, 0.0f), roomsParent);
        //roomGO.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    void CreateCorridors(int corridorMaxSize, Vector2 origin, Vector2 direction)
    {
        if ((int)origin.y < 0 || (int)origin.y >= mapElements[0].Length || (int)origin.x < 0 || (int)origin.x >= mapElements[0].Length) return;
        List<Vector2> newCorridors = new List<Vector2>(); // The corridors created from the origin
        //int corridorSize = InitNewValue(corridorMaxSize, Gap.corridor);

        // Start at one as origin already exists
        for (int i = 1; i < corridorMaxSize; i++)
        {
            Vector2 position = origin + (direction * i);

            // Check if corridor is out of index
            if ((int)position.x > 0 && (int)position.x < mapMaxSize && (int)position.y > 0 && (int)position.y < mapMaxSize)
                mapElements[(int)position.y][(int)position.x] = (int)GenerationElement.Corridor;

            newCorridors.Add(position);

            // If a corridor already exists there then it's an intersection, stop the corridor here
            if (IsAnIntersection(position, direction)) break;
        }

        CreateSources(corridorMaxSize, direction, newCorridors);
    }

    void CreateSources(int corridorMaxSize, Vector2 direction, List<Vector2> newCorridors)
    {
        List<Vector2> corridorsSources = new List<Vector2>(); // The corridors choosed from the newCorridors to create new origin

        // Create a new origin each 5 - 10 corridors
        for (int i = Random.Range(5, 10); i < newCorridors.Count; i += Random.Range(5, 10))
        {
            corridorsSources.Add(newCorridors[i]);
        }

        foreach (Vector2 corridorSource in corridorsSources)
        {
            Vector2 newDirection = GetNewDirection(direction); // Random direction perpendicural to direction

            CreateCorridors(InitNewValue(corridorMaxSize / 2, Gap.corridor), corridorSource, newDirection); // Recursive
        }
    }

    bool IsAnElement(GenerationElement element, Vector2 position)
    {
        if ((int)position.x > 0 && (int)position.x < mapMaxSize && (int)position.y > 0 && (int)position.y < mapMaxSize)
        {
            if (mapElements[(int)position.y][(int)position.x] == (int)element)
            {
                return true;
            }
        }

        return false;
    }

    // Overload for Doors to prevent calling two times the function
    bool IsAnElement(GenerationElement element, GenerationElement secondElement, Vector2 position)
    {
        if ((int)position.x > 0 && (int)position.x < mapMaxSize && (int)position.y > 0 && (int)position.y < mapMaxSize)
        {
            if (mapElements[(int)position.y][(int)position.x] == (int)element || mapElements[(int)position.y][(int)position.x] == (int)secondElement)
            {
                return true;
            }
        }

        return false;
    }

    // NEED OPTIMISATION
    // Check the surroundings of the new created corridor to prevent a two layer corridor
    bool IsAnIntersection(Vector2 position, Vector2 direction)
    {
        // Create the four even if only three will be use to prevent long lines of code
        bool doubleCorridorUp = IsAnElement(GenerationElement.Corridor, position + Vector2.up);
        bool doubleCorridorDown = IsAnElement(GenerationElement.Corridor, position + Vector2.down);
        bool doubleCorridorLeft = IsAnElement(GenerationElement.Corridor, position + Vector2.left);
        bool doubleCorridorRight = IsAnElement(GenerationElement.Corridor, position + Vector2.right);

        switch (direction)
        {
            case Vector2 v when v.Equals(Vector2.down):
                return (doubleCorridorDown || doubleCorridorLeft || doubleCorridorRight);
            case Vector2 v when v.Equals(Vector2.up):
                return (doubleCorridorUp || doubleCorridorLeft || doubleCorridorRight);
            case Vector2 v when v.Equals(Vector2.left):
                return (doubleCorridorUp || doubleCorridorLeft || doubleCorridorDown);
            case Vector2 v when v.Equals(Vector2.right):
                return (doubleCorridorUp || doubleCorridorRight || doubleCorridorDown);
            default:
                return false;
        }
    }

    void CreateCorridorsFromMap()
    {
        for (int y = 0; y < mapElements[0].Length; y++)
        {
            for (int x = 0; x < mapElements[0].Length; x++)
            {
                if (mapElements[y][x] == (int)GenerationElement.Corridor)
                {
                    CreateCorridor(new Vector2(x, y));
                }
            }
        }
    }

    void CreateCorridor(Vector2 position)
    {
        bool doubleCorridorUp = IsAnElement(GenerationElement.Corridor, GenerationElement.Door, position + Vector2.up);
        bool doubleCorridorDown = IsAnElement(GenerationElement.Corridor, GenerationElement.Door, position + Vector2.down);
        bool doubleCorridorLeft = IsAnElement(GenerationElement.Corridor, GenerationElement.Door, position + Vector2.left);
        bool doubleCorridorRight = IsAnElement(GenerationElement.Corridor, GenerationElement.Door, position + Vector2.right);
        Corridor _corridor = Corridor.DeadEnd; // Default value in switch
        float rotationY = 0.0f;
        Quaternion rotation = Quaternion.identity;
        GameObject prefab = prefabCorridor[(int)Corridor.Straight];

        switch (doubleCorridorUp, doubleCorridorDown, doubleCorridorLeft, doubleCorridorRight)
        {
            case (true, true, true, true):
                _corridor = Corridor.Star;
                break;
            case (true, false, true, true):
                _corridor = Corridor.ThreeIntersections;
                rotationY = -90.0f;
                break;
            case (false, true, true, true):
                _corridor = Corridor.ThreeIntersections;
                rotationY = 90.0f;
                break;
            case (true, true, true, false):
                _corridor = Corridor.ThreeIntersections;
                rotationY = 180.0f;
                break;
            case (true, true, false, true):
                _corridor = Corridor.ThreeIntersections;
                break;
            case (false, false, true, true):
                _corridor = Corridor.Straight;
                rotationY = 90.0f;
                break;
            case (true, true, false, false):
                _corridor = Corridor.Straight;
                break;
            case (true, false, true, false):
                _corridor = Corridor.Corner;
                rotationY = 90.0f;
                break;
            case (true, false, false, true):
                _corridor = Corridor.Corner;
                rotationY = 180.0f;
                break;
            case (false, true, true, false):
                _corridor = Corridor.Corner;
                break;
            case (false, true, false, true):
                _corridor = Corridor.Corner;
                rotationY = -90.0f;
                break;
            default:
                if (doubleCorridorLeft)
                    rotationY = 90.0f;
                else if (doubleCorridorRight)
                    rotationY = -90.0f;
                else if (doubleCorridorUp)
                    rotationY = -180.0f;
                break;
        }

        prefab = prefabCorridor[(int)_corridor];
        rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
        Instantiate(prefab, new Vector3(position.x * offsetSize, 0.0f, position.y * offsetSize), rotation, corridorsParent);
        TrySpawnElements(position, rotation, _corridor);
    }

    void TrySpawnElements(Vector2 position, Quaternion rotation, Corridor _corridor)
    {
        for (int i = 0; i < schoolElements.Length; i++)
        {
            SchoolElement element = schoolElements[i];

            foreach (Corridor corridor in element.corridors)
            {
                if (corridor == _corridor && Random.Range(0, 100) <= element.probability && (element.maxNumber == 0 || element.number < element.maxNumber))
                {
                    Instantiate(element.prefab, new Vector3(position.x * offsetSize, 0.0f, position.y * offsetSize), rotation, schoolElementsParent);
                    schoolElements[i].number++;
                    return;
                }
            }
        }
    }

    Vector2 GetNewDirection(Vector2 oldDirection)
    {
        if (oldDirection == Vector2.up || oldDirection == Vector2.down)
        {
            return ProbabilityIsCorrect(Probability.direction) ? Vector2.left : Vector2.right;
        }
        else
        {
            return ProbabilityIsCorrect(Probability.direction) ? Vector2.up : Vector2.down;
        }
    }

    int InitNewValue(int initialValue, Gap possibleGap)
    {
        int newValue = Random.Range(initialValue - (int)possibleGap, initialValue + (int)possibleGap);

        return newValue;
    }

    bool ProbabilityIsCorrect(Probability probability)
    {
        int resultProbability = Random.Range(1, (int)probability);

        return (resultProbability == 1); // Chance is 1 / probability
    }

    void InitDoubleArray(int size)
    {
        mapElements = new int[size][];

        for (int i = 0; i < size; i++)
        {
            mapElements[i] = new int[size];
        }
    }

    void PrintDoubleArray(int[][] array)
    {
        for (int i = 0; i < array[0].Length; i++)
        {
            string elements = "";

            for (int y = 0; y < array[0].Length; y++)
            {
                elements += array[i][y].ToString();
            }

            //Debug.Log(elements);
        }
    }

    void MoveCameraToCenter()
    {

        GameObject camera = GameObject.Find("Main Camera");
        if (camera)
            camera.transform.position = new Vector3(mapMaxSize / 2 * offsetSize, mapMaxSize * offsetSize, mapMaxSize / 2 * offsetSize);
    }
}
