using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public GridController targetPoint, playerPoint;
    public Dictionary<string, GridController> movableGrids = new Dictionary<string, GridController>();

    public PlayerController player;
    public int x, y, z;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SendRayCast();
        }
        if(Input.GetMouseButtonDown(1))
        {
            PlayerMove(targetPoint);
        }
    }

    public void SendRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var gridController = hit.transform.GetComponent<GridController>();
            Debug.Log(hit.transform.gameObject.name);

            if (player == null && gridController.myObject.myData.myType == ObjectTypeData.Type.PLAYER)
            {
                playerPoint = gridController;
                player = gridController.myObject.GetComponent<PlayerController>();
                FindMovableGrids(gridController);
            }
            else if (player != null && gridController.myObject == null)
            {
                if(CheckMovableGrid(gridController) == true)
                {
                    if (targetPoint != null)
                    {
                        targetPoint.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Blue");
                    }
                    targetPoint = gridController;
                    targetPoint.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Pink");
                }
                else
                {
                    Debug.Log("이동 불가 타일");
                }
            }
        }
    }

    public void PlayerMove(GridController grid)
    {
        Vector3 destination = new Vector3(grid.x, 1.5f, grid.z);
        Debug.Log(destination);

        float speed = 5.0f;
        float stoppingDistance = 0.01f;

        playerPoint.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Red");
        foreach(GridController movable in movableGrids.Values)
        {
            movable.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Red");
        }
        StartCoroutine(MoveToDestination(destination, speed, stoppingDistance));
    }

    private IEnumerator MoveToDestination(Vector3 destination, float speed, float stoppingDistance)
    {
        while (Vector3.Distance(player.transform.position, destination) > stoppingDistance)
        {
            float step = speed * Time.deltaTime;
            player.transform.position = Vector3.Lerp(player.transform.position, destination, step);
            yield return null;
        }

        player.transform.position = destination;

        targetPoint.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Red");

        player.GetComponent<ObjectManager>().GetMyPosition();
        movableGrids.Clear();
        targetPoint = null;
        playerPoint = null;
        player = null;

    }


    public void FindMovableGrids(GridController grid)
    {
        SetMovableGrid(grid, 0, 1, 1, "left");
        SetMovableGrid(grid, 0, -1, 1, "right");
        SetMovableGrid(grid, -1, 0, 1, "front");
        SetMovableGrid(grid, 1, 0, 1, "back");
        SetMovableGrid(grid, 1, 1, 2, "diagonalLB");
        SetMovableGrid(grid, 1, -1, 2, "diagonalRB");
        SetMovableGrid(grid, -1, 1, 2, "diagonalLF");
        SetMovableGrid(grid, -1, -1, 2, "diagonalRF");
    }

    private void SetMovableGrid(GridController grid, int xOffset, int zOffset, int distance, string destination)
    {
        string key = $"{grid.x + xOffset}, {grid.z + zOffset}";
        GridController targetGrid = GridManager.Instance.gridDictionary[key];

        string movableKey = $"{destination}, {distance}";

        if (targetGrid != null && (targetGrid.myObject == null || targetGrid.myObject.myData.myType != ObjectTypeData.Type.OBSTACLE))
        {
            movableGrids.Add(movableKey, targetGrid);
            if(distance == 1)
            {
                targetGrid.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Blue");
            }
            else if(distance == 2)
            {
                targetGrid.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Orange");
            }
        }
    }

    private bool CheckMovableGrid(GridController grid)
    {
        foreach (GridController movable in movableGrids.Values)
        {
            if (grid == movable)
            {
                Debug.Log(movable.name);
                return true;
            }
        }

        return false;
    }
}
