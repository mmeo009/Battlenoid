using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class CubeGrid
{
    public CubeGrid(int _x, int _y, int _z, bool hasObject)
    {
        x = _x;
        y = _y;
        z = _z;

        if (hasObject)
        {
            ac = accessibility.IMPOSSIBLE;
        }
        else
        {
            ac = _y != 0 ? accessibility.TRICKY : accessibility.MOVABLE;
        }
    }

    public enum accessibility
    {
        MOVABLE,
        IMPOSSIBLE,
        TRICKY,
        DEFAULT
    }

    public enum direction
    {
        left,
        right,
        front,
        back,
        none
    }

    public enum myObject
    {
        player,
        none,
        other,
    }

    public accessibility ac = accessibility.DEFAULT;
    public direction dir = direction.none;
    public myObject mo = myObject.none;

    public CubeGrid ParentCube;
    public int x, y, z;

    public int G, H;
    public int F { get { return G + H; } }

}
[System.Serializable]
public class Stage
{
    public Stage(int _stageNum, int _maxX, int _maxY, int _maxZ) { stageNum = _stageNum; maxX = _maxX; maxY = _maxY; maxZ = _maxZ; }
    public int stageNum;
    public int maxX, maxY, maxZ;
}

[System.Serializable]
public class ObjectData
{
    public ObjectData(int _x, int _y, int _z, Type _type, GameObject _me) { x = _x; y = _y; z = _z; myType = _type; me = _me; }
    public enum Type
    {
        OBSTACLE,
        PLAYER,
        BOUNDARY,
        CUBEGRID,
        DEFAULT
    }

    public int x, y, z;
    public Type myType = Type.DEFAULT;
    public PlayerController player;

    public UnityEngine.GameObject me;
}

public class ObjectManager : MonoBehaviour
{
    public Vector3Int cubeX, cubeY, cubeZ, startPos, targetPos;
    public List<CubeGrid> finalPath;

    public int mapSizeX, mapSizeY, mapSizeZ;
    public CubeGrid[,] cubeArray;
    public Dictionary<string, ObjectData> objectDictionary = new Dictionary<string, ObjectData>();
    public CubeGrid startCube, targetCube, curCube;
    public List<CubeGrid> open, closed;

    public Stage stage = new Stage(1, 15, 0, 20);

    public static ObjectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            transform.parent = null;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        CreatMap();
    }

    public void CreatMap()
    {
        mapSizeX = stage.maxX;
        mapSizeY = stage.maxY;
        mapSizeZ = stage.maxZ;
        cubeArray = new CubeGrid[mapSizeX, mapSizeZ];
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeZ; j++)
            {
                bool hasObject = false;
                cubeArray[i, j] = new CubeGrid(i, 0, j, hasObject);
                CreateObject(i, 0, j, $"Stage_{stage.stageNum}_Cube");
            }
        }
        CreateObject(0, 1, 0, "Player");
    }

    public void PathFinding(Vector3Int startPos, Vector3Int targetPos)
    {
        startCube = cubeArray[startPos.x, startPos.z];
        targetCube = cubeArray[targetPos.x, targetPos.z];

        open = new List<CubeGrid>() { startCube };
        closed = new List<CubeGrid>();
        finalPath = new List<CubeGrid>();
        finalPath.Clear();
        

        while (open.Count > 0)
        {
            curCube = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].F <= curCube.F || open[i].H < curCube.H)
                {
                    curCube = open[i];
                }
            }
            open.Remove(curCube);
            closed.Add(curCube);

            if (curCube == targetCube)
            {
                CubeGrid TargetCurCube = targetCube;
                while (TargetCurCube != startCube)
                {
                    finalPath.Add(TargetCurCube);
                    TargetCurCube = TargetCurCube.ParentCube;
                }
                finalPath.Add(startCube);
                finalPath.Reverse();

                for (int i = 0; i < finalPath.Count; i++)
                {
                    print(i + "번째는 " + finalPath[i].x + ", " + finalPath[i].z + ", 방향: " + finalPath[i].dir);
                }
                MoveStart();
                return;
            }

            OpenListAdd(curCube.x, curCube.z + 1, CubeGrid.direction.front);
            OpenListAdd(curCube.x + 1, curCube.z, CubeGrid.direction.right);
            OpenListAdd(curCube.x, curCube.z - 1, CubeGrid.direction.back);
            OpenListAdd(curCube.x - 1, curCube.z, CubeGrid.direction.left);
        }

        print("경로를 찾을 수 없습니다.");
    }
    void OpenListAdd(int checkX, int checkZ, CubeGrid.direction dir)
    {
        if (checkX >= 0 && checkX < mapSizeX && checkZ >= 0 && checkZ < mapSizeZ
            && cubeArray[checkX, checkZ].ac != CubeGrid.accessibility.IMPOSSIBLE
            && !closed.Contains(cubeArray[checkX, checkZ]))
        {
            CubeGrid NeighborCube = cubeArray[checkX, checkZ];
            int MoveCost = curCube.G + ((curCube.x - checkX == 0 || curCube.z - checkZ == 0) ? 10 : 14);

            if (MoveCost < NeighborCube.G || !open.Contains(NeighborCube))
            {
                NeighborCube.G = MoveCost;
                NeighborCube.H = (Mathf.Abs(NeighborCube.x - targetCube.x) + Mathf.Abs(NeighborCube.z - targetCube.z)) * 10;
                NeighborCube.ParentCube = curCube;
                NeighborCube.dir = dir;

                if (!open.Contains(NeighborCube))
                {
                    open.Add(NeighborCube);
                    open.Sort((a, b) => a.F.CompareTo(b.F)); // F 값 기준으로 리스트 정렬
                }
            }
        }
    }

    public void CreateObject(int _x , int _y, int _z , string objectId)
    {
        string path = $"Prefabs/{objectId}";
        ObjectData.Type _type = ObjectData.Type.DEFAULT;
        GameObject temp = (GameObject)Instantiate(Resources.Load(path));
        temp.transform.position = new Vector3Int(_x, _y, _z);
        temp.name = $"{objectId}_{_x},{_y},{_z}";

        if (objectId.Contains("Player"))
        {
            temp.AddComponent<PlayerController>();
            _type = ObjectData.Type.PLAYER;
        }
        else if(objectId.Contains("Cube"))
        {
            _type = ObjectData.Type.CUBEGRID;
        }
        
        ObjectData objectData = new ObjectData(_x, _y, _z, _type, temp);

        objectDictionary.Add($"{_x},{_y},{_z}", objectData);
        FindObjectsCube(_x, _y, _z, objectId);
    }

    public void FindObjectsCube(int _x, int _y, int _z , string objectId)
    {
        if(!objectId.Contains("Cube"))
        {
            ObjectData.Type _type = objectDictionary[$"{_x},{_y},{_z}"].myType;
            Debug.Log(_type);
            if(_type == ObjectData.Type.PLAYER)
            {
                cubeArray[_x, _z].mo = CubeGrid.myObject.player;
                Debug.Log(cubeArray[_x, _z].mo);
            }
        }
    }
    public void MoveStart()
    {
        if (finalPath.Count < 3)
        {
            for (int i = 0; i < finalPath.Count; i++)
            {
                if (i == 0)
                {
                    Vector3Int start = new Vector3Int(startCube.x, startCube.y, startCube.z);
                    PlayerMove(start, finalPath[i]);
                }
                else
                {
                    Vector3Int pos = new Vector3Int(finalPath[i - 1].x, finalPath[i - 1].y, finalPath[i - 1].z);
                    PlayerMove(pos, finalPath[i]);
                }
            }
        }
    }
    public void PlayerMove(Vector3Int startPos, CubeGrid targetGrid)
    {
        int angle = 0;
        if (targetGrid.dir == CubeGrid.direction.left)
        {
            angle = 270;
        }
        else if (targetGrid.dir == CubeGrid.direction.back)
        {
            angle = 180;
        }
        else if (targetGrid.dir == CubeGrid.direction.right)
        {
            angle = 90;
        }
        StartCoroutine(MoveToDestination(startPos, targetGrid, 5.0f, 0.01f, angle));
    }

    private IEnumerator MoveToDestination(Vector3 playerPos, CubeGrid targetGrid, float speed, float stoppingDistance, int angle)
    {
        ObjectData playerData = objectDictionary[$"{playerPos.x},{playerPos.y + 1},{playerPos.z}"];

        GameObject player = playerData.me;
        Vector3 destination = new Vector3(targetGrid.x, targetGrid.y + 1, targetGrid.z);

        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);

        bool rotationComplete = false;

        playerData.x = targetGrid.x;
        playerData.y = targetGrid.y + 1;
        playerData.z = targetGrid.z;
        cubeArray[(int)playerPos.x, (int)playerPos.z].mo = CubeGrid.myObject.none;
        cubeArray[(int)targetGrid.x, (int)targetGrid.z].mo = CubeGrid.myObject.player;
        objectDictionary.Add($"{targetGrid.x},{targetGrid.y + 1},{targetGrid.z}", playerData);
        objectDictionary[$"{playerPos.x},{playerPos.y},{playerPos.z}"].player = null;
        objectDictionary.Remove($"{playerPos.x},{playerPos.y + 1},{playerPos.z}");

        while (Vector3.Distance(player.transform.position, destination) > stoppingDistance || !rotationComplete)
        {
            if (!rotationComplete)
            {
                player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, speed * Time.deltaTime);

                if (Quaternion.Angle(player.transform.rotation, targetRotation) < 1.0f)
                {
                    rotationComplete = true;
                }
            }
            else
            {
                float step = speed * Time.deltaTime;
                player.transform.position = Vector3.Lerp(player.transform.position, destination, step);
            }

            yield return null;
        }
        player.transform.position = destination;
        player.transform.rotation = targetRotation;

    }
}