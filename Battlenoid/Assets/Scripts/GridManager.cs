using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            transform.parent = null;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        SetData();
    }

    public GridController[] grids;
    public Dictionary<string, GridController> gridDictionary = new Dictionary<string, GridController>();

    public void SetData()
    {
        foreach(GridController _grid in grids)
        {
            string pos = $"{_grid.x}, {_grid.z}";
            gridDictionary.Add(pos, _grid);
        }
    }
}
