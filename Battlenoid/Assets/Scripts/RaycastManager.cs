using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public Vector3Int targetPoint, playerPoint;
    public ObjectManager objectManager;
    public PlayerController player;
    public int x, y, z;

    private void Start()
    {
        objectManager = ObjectManager.Instance;
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SendRayCast();
        }
        if(Input.GetMouseButtonDown(1))
        {
            objectManager.PathFinding(playerPoint, targetPoint);
            player = null;
        }
    }

    public void SendRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3Int _targetPoint = Vector3Int.RoundToInt(hit.transform.position);
            Debug.Log(_targetPoint);

            CubeGrid cube = objectManager.cubeArray[_targetPoint.x, _targetPoint.z];
            if (player == null && cube.mo == CubeGrid.myObject.player)
            {

                playerPoint = _targetPoint;
                player = objectManager.objectDictionary[$"{_targetPoint.x},{_targetPoint.y + 1},{_targetPoint.z}"].me.GetComponent<PlayerController>();
            }
            else if (player != null && objectManager.cubeArray[_targetPoint.x, _targetPoint.z].mo == CubeGrid.myObject.none)
            {
                targetPoint = _targetPoint;
            }
        }
    }
}
