using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public int x, y, z, degree;
    public ObjectManager myObject;
    private void Awake()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.y;
        z = (int)transform.position.z;
        this.gameObject.name = $"Grid : {x},{y},{z}";
    }
}
