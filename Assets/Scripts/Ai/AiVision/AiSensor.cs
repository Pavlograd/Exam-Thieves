using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//video: https://www.youtube.com/watch?v=znZXmmyBF-o 9:35

[ExecuteInEditMode]
public class AiSensor : MonoBehaviour
{
    public AiAgentConfig config;
    public Color meshColor = Color.red;
    public LayerMask layers;
    public LayerMask occlusionLayers;
    public List<GameObject> Objects
    {
        get
        {
            objects.RemoveAll(obj => !obj);
            return objects;
        }
    }
    private List<GameObject> objects = new List<GameObject>();
    Collider[] colliders = new Collider[50];
    Mesh mesh;
    int count;
    float scanInterval;
    float scanTimer;

    // Start is called before the first frame update
    void Start()
    {
        scanInterval = 1.0f / config.scanFrequency;
        //Debug.Log(DifficultySettings.datas.FOVGuards);
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;

        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, DifficultySettings.datas.FOVGuards, colliders, layers, QueryTriggerInteraction.Collide);
        objects.Clear();

        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj) || isNear(obj))
            {
                Objects.Add(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        Vector3 offset = new Vector3(0f, 1f, 0f);
        if (/*direction.y < 0* ||*/ direction.y > config.height)
        {

            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);

        if (deltaAngle > config.angle)
        {
            return false;
        }

        origin.y += (config.height / 2);
        dest.y = origin.y;

        if (obj.GetComponent<PlayerMotor>().getIsCrounching())
        {
            Debug.DrawLine(origin, dest, Color.blue);
            if (Physics.Linecast(origin - offset, dest - offset, occlusionLayers))
            {
                Debug.DrawLine(origin, dest, Color.red);
                return false;
            }

        }
        else
        {
            Debug.DrawLine(origin, dest, Color.green);
            if (Physics.Linecast(origin, dest, occlusionLayers))
            {
                return false;
                Debug.DrawLine(origin, dest, Color.yellow);
            }
        }
        /*if(Physics.Linecast(origin, dest, occlusionLayers))
        {
            return false;
            Debug.DrawLine(origin, dest, Color.blue);
        }
        else
        {
            Debug.DrawLine(origin, dest, Color.red);
        }*/


        return true;

    }
    public bool isNear(GameObject obj)
    {
        if(Vector3.Distance(obj.transform.position, this.transform.position) >= 2f)
        {
            return false;
        }
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        Vector3 offset = new Vector3(0f, 1f, 0f);
        direction.y = 0;
        origin.y += (config.height / 2);
        dest.y = origin.y;

        if (obj.GetComponent<PlayerMotor>().getIsCrounching())
        {
            Debug.DrawLine(origin, dest, Color.blue);
            if (Physics.Linecast(origin - offset, dest - offset, occlusionLayers))
            {
                Debug.DrawLine(origin, dest, Color.red);
                return false;
            }

        }
        else
        {
            Debug.DrawLine(origin, dest, Color.green);
            if (Physics.Linecast(origin, dest, occlusionLayers))
            {
                return false;
                Debug.DrawLine(origin, dest, Color.yellow);
            }
        }
        return true;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -(config.angle), 0) * Vector3.forward * DifficultySettings.datas.FOVGuards;
        Vector3 bottomRight = Quaternion.Euler(0, config.angle, 0) * Vector3.forward * DifficultySettings.datas.FOVGuards;

        Vector3 topCenter = bottomCenter + Vector3.up * config.height;
        Vector3 topRight = bottomRight + Vector3.up * config.height;
        Vector3 topLeft = bottomLeft + Vector3.up * config.height;

        int vert = 0;

        //left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;
        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -config.angle;
        float deltaAngle = (config.angle * 2) / segments;

        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * DifficultySettings.datas.FOVGuards;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * DifficultySettings.datas.FOVGuards;

            topRight = bottomRight + Vector3.up * config.height;
            topLeft = bottomLeft + Vector3.up * config.height;

            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;
            //bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / config.scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, DifficultySettings.datas.FOVGuards);
        for (int i = 0; i < count; ++i)
        {
            Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in objects)
        {
            Gizmos.DrawSphere(obj.transform.position, 0.2f);
        }
    }

    public int Filter(GameObject[] buffer, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        int count = 0;

        foreach (var obj in Objects)
        {
            if (obj.layer == layer)
            {
                buffer[count++] = obj;
            }

            if (buffer.Length == count)
            {
                break; //buffer is full
            }
        }
        return count;
    }
}
