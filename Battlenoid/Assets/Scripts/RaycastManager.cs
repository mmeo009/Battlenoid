using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public GridController targetPoint;
    public GridController playerPoint;
    public PlayerController player;
    public int x, y, z;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SendRayCast();
        }
        if(Input.GetMouseButtonUp(0))
        {
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
                gridController.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Blue");
            }
            else if (player != null && (gridController.myObject == null || gridController.myObject.myData.myType != ObjectTypeData.Type.PLAYER ))
            {
                if(targetPoint != null)
                {
                    targetPoint.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Red");
                }
                targetPoint = gridController;
                gridController.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Pink");
            }
            else if(player != null && gridController.myObject.myData.myType == ObjectTypeData.Type.PLAYER)
            {
                targetPoint = null;
                playerPoint = gridController;
                player = gridController.myObject.GetComponent<PlayerController>();
                gridController.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Blue");
            }

        }
    }
}
