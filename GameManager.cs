using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour
{
 
    private string mapFilePath = "Assets/Maps/map1";
    private MapData mapData;
    private MapController mapController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /*
    public void LoadMap()
    {
        mapData = Resources.Load<MapData>(mapFilePath);
        mapController = gameObject.AddComponent<MapController>();
    }

    public void LoadMapIntoGame()
    {
        List<GameObject> ControlPoints = new();

        foreach (var point in mapData.ControlPoints)
        {
            ControlPoints.Add(point);
        }

        foreach (var point in ControlPoints)
        {
            Instantiate(point, this.transform);
        }
    }
    */
}
