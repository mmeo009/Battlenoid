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
        GetMyPosition(0);
    }
    public void CheckMyType()
    {
        Component pc = GetComponent<PlayerController>();
        if(pc != null)
        {
            myData.myType = ObjectTypeData.Type.PLAYER;
        }

    }
    public void GetMyPosition(int type, Grid grid = null)
    {
        if (type == 0)
        {
            int x = (int)transform.position.x;
            int z = (int)transform.position.z;

            GridController[] grids = FindObjectsOfType<GridController>();
            foreach (GridController gridController in grids)
            {
                if (gridController.x == x && gridController.z == z)
                {
                    myPosition = gridController;
                    gridController.myObject = this;
                }
            }

            if (myPosition == null)
            {
                return;
            }
        }
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
