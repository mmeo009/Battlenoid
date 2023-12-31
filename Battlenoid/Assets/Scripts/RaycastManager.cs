using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public GridController targetPoint, playerPoint;
    public GridController movableLeft, movableRight, movableFront, movableBack;

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
        movableBack.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Red");
        movableFront.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Red");
        movableLeft.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Red");
        movableRight.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Red");
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

        player.GetComponent<ObjectManager>().GetMyPosition(0);

        targetPoint = null;
        playerPoint = null;
        player = null;
        movableBack = null;
        movableFront = null;
        movableLeft = null;
        movableRight = null;
    }


    public void FindMovableGrids(GridController grid)
    {
        SetMovableGrid(grid, 0, 1, ref movableLeft);
        SetMovableGrid(grid, 0, -1, ref movableRight);
        SetMovableGrid(grid, -1, 0, ref movableFront);
        SetMovableGrid(grid, 1, 0, ref movableBack);
    }

    private void SetMovableGrid(GridController grid, int xOffset, int zOffset, ref GridController movableGrid)
    {
        string key = $"{grid.x + xOffset}, {grid.z + zOffset}";
        GridController targetGrid = GridManager.Instance.gridDictionary[key];

        if (targetGrid != null && (targetGrid.myObject == null || targetGrid.myObject.myData.myType != ObjectTypeData.Type.OBSTACLE))
        {
            movableGrid = targetGrid;
            movableGrid.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Blue");
        }
    }

    private bool CheckMovableGrid(GridController grid)
    {
        if(grid == movableLeft || grid == movableRight || grid == movableFront || grid == movableBack)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
