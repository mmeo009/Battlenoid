using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPositionFinder : MonoBehaviour
{
    public ObjectData ObjectData;
    private void Start()
    {
        if(this.ObjectData.x == 0 || this.ObjectData.y == 0 || this.ObjectData.z == 0)
        {
            this.ObjectData.x = (int)this.transform.position.x;
            this.ObjectData.y = (int)this.transform.position.y;
            this.ObjectData.z = (int)this.transform.position.z;
        }
        if(ObjectData.me == null)
        {
            ObjectData.me = this.gameObject;
        }
        if(ObjectManager.Instance.objectDictionary.ContainsKey($"{ObjectData.x},{ObjectData.y},{ObjectData.z}") == false)
        {
            ObjectManager.Instance.objectDictionary.Add($"{ObjectData.x},{ObjectData.y},{ObjectData.z}", ObjectData);
        }

        if (this.ObjectData.myType == ObjectData.Type.CUBEGRID && ObjectManager.Instance.cubeArray[this.ObjectData.x, this.ObjectData.z] == null)
        {
            ObjectManager.Instance.cubeArray[this.ObjectData.x, this.ObjectData.z] = new CubeGrid(this.ObjectData.x, 0, this.ObjectData.z, false);
        }
        Destroy(this);
    }
}
