using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    private MapData mapData;
    private MapController mapController;
    private PlayerController playerController;
    private PrefabController prefabController;



    // Start is called before the first frame update
    void Start()
    {
        prefabController = new PrefabController();
        GameObject playerBall = prefabController.BallPrefabs[3];
        GameObject mapController = prefabController.MapController;
        Instantiate(mapController, this.transform).AddComponent<BallQueue>();
        Instantiate(playerBall, this.transform.GetChild(0)).AddComponent<PlayerController>();

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
