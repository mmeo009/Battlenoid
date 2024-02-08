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
        ObjectManager.Instance.objectDictionary.Add($"{ObjectData.x},{ObjectData.y},{ObjectData.z}", ObjectData);
        Destroy(this);
    }
}
