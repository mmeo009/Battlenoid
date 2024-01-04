using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GridManager gridManager;
    public GridController myPosition;
    public Vector3Int myPos;
    public ObjectTypeData myData = new ObjectTypeData();

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            CheckMyType();
            Invoke("GetMyPosition", 1.5f);
        }
    }
    public void CheckMyType()
    {
        gridManager = GridManager.Instance;
        Component pc = GetComponent<PlayerController>();
        if(pc != null)
        {
            myData.myType = ObjectTypeData.Type.PLAYER;
        }

    }
    public void GetMyPosition()
    {
        myPos = Vector3Int.RoundToInt(transform.position);
        if(myData.myType != ObjectTypeData.Type.DEFAULT)
        {
            if(myData.myType == ObjectTypeData.Type.PLAYER)
            {
                gridManager.cubeArray[myData.x, myData.z] = new CubeGrid(myPos.x, myPos.y - 2, myPos.z, false);
            }
            else
            {
                gridManager.cubeArray[myData.x, myData.z] = new CubeGrid(myPos.x, myPos.y, myPos.z, true);
            }
        }

        string key = $"{myPos.x}, {myPos.z}";

        GridController targetGrid = gridManager.gridDictionary[key];
        if (myPosition != null)
        {
            myPosition.myObject = null;
        }
        myPosition = targetGrid;
        targetGrid.myObject = this;
    }
}

public class ObjectTypeData
{
    public enum Type
    {
        OBSTACLE,
        PLAYER,
        BOUNDARY,
        DEFAULT
    }

    public int x, y, z;
    public Type myType = Type.DEFAULT;
}
