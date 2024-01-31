using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public Vector3Int targetPoint, playerPoint;
    MeshRenderer targetMesh;
    public ObjectManager objectManager;
    public PlayerController player;
    public enum Mode
    {
        Move,
        Attack,
        None
    }
    public Mode playerMode = Mode.None;
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
            if(playerMode == Mode.Move)
            {
                objectManager.PathFinding(playerPoint, targetPoint);
            }
            else if(playerMode == Mode.Attack)
            {

            }
            targetMesh.materials[0] = (Material)Resources.Load("Materials/New Material");
            player = null;
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            if (playerMode == Mode.None || playerMode == Mode.Attack)
            {
                playerMode = Mode.Move;
            }
            else if( playerMode == Mode.Move)
            {
                playerMode = Mode.Attack;
            }
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
                targetMesh = objectManager.objectDictionary[$"{_targetPoint.x},{_targetPoint.y},{_targetPoint.z}"].me.GetComponent<MeshRenderer>();
                Debug.Log(targetMesh.gameObject);
                Debug.Log(targetMesh.materials[0].name);
                Material _cube = (Material)Resources.Load("Shader/VFX/Emphasis/Cube");
                Debug.Log(_cube.name);
                targetMesh.materials[0] = _cube;
                Debug.Log(targetMesh.materials[0].name);
            }
        }
    }
}
