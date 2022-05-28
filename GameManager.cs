using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    private PrefabController prefabController;

    // Start is called before the first frame update
    void Start()
    {
        prefabController = new PrefabController();
        
        GameObject mapController = prefabController.mapController;
        Instantiate(mapController, this.transform).AddComponent<MapController>();

        Position p = new Position(prefabController.route, 0.5f, 1);

        Debug.Log(p.totalCurves);

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
