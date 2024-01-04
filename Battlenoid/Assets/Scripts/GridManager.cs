using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeGrid
{
    public CubeGrid (int _x, int _y, int _z, bool hasObject)
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

    public accessibility ac = accessibility.DEFAULT;
    public direction dir = direction.none;

    public CubeGrid ParentCube;
    public int x, y, z;

    public int G, H;
    public int F { get {return G + H; } }

}
[System.Serializable]
public class Stage
{
    public Stage(int _stageNum, int _maxX, int _maxY, int _maxZ) { stageNum = _stageNum; maxX = _maxX; maxY = _maxY; maxZ = _maxZ; }
    public int stageNum;
    public int maxX, maxY, maxZ;
}
public class GridManager : MonoBehaviour
{
    public Vector3Int cubeX, cubeY, cubeZ, startPos, targetPos;
    public List<CubeGrid> finalPath;

    public int mapSizeX, mapSizeY, mapSizeZ;
    public CubeGrid[,] cubeArray;
    public CubeGrid startCube, targetCube, curCube;
    public List<CubeGrid> open, closed;

    public Stage stage = new Stage(1, 20, 0, 20);

    public static GridManager Instance { get; private set; }

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
        SetData();
        CreatMap();
    }

    public void CreatMap()
    {
        mapSizeX = stage.maxX;
        mapSizeY = stage.maxY;
        mapSizeZ = stage.maxZ;

        cubeArray = new CubeGrid[mapSizeX, mapSizeZ];
        for(int i = 0; i < mapSizeX; i ++)
        {
            for(int j = 0; j < mapSizeZ; j ++)
            {
                bool hasObject = false;
                cubeArray[i, j] = new CubeGrid(i, 0, j, hasObject);
            }
        }
    }

    public void PathFinding(Vector3Int startPos, Vector3Int targetPos)
    {
        startCube = cubeArray[startPos.x, startPos.z];
        targetCube = cubeArray[targetPos.x, targetPos.z];

        open = new List<CubeGrid>() { startCube };
        closed = new List<CubeGrid>();
        finalPath = new List<CubeGrid>();

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

    public GridController[] grids;
    public Dictionary<string, GridController> gridDictionary = new Dictionary<string, GridController>();

    public void SetData()
    {
        foreach(GridController _grid in grids)
        {
            string pos = $"{_grid.x}, {_grid.z}";
            gridDictionary.Add(pos, _grid);
        }
    }
}
