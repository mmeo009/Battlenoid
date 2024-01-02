using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GridController myPosition;
    public ObjectTypeData myData = new ObjectTypeData();

    public void Start()
    {
        CheckMyType();
        Invoke("GetMyPosition", 1.5f);
    }
    public void CheckMyType()
    {
        Component pc = GetComponent<PlayerController>();
        if(pc != null)
        {
            myData.myType = ObjectTypeData.Type.PLAYER;
        }

    }
    public void GetMyPosition()
    {
        int x = (int)transform.position.x;
        int z = (int)transform.position.z;
        string key = $"{x}, {z}";

        GridController targetGrid = GridManager.Instance.gridDictionary[key];
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
