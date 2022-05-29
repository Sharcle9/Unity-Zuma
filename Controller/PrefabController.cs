using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabController
{
    private string prefabFolderPath = "Prefabs/";
    public GameObject[] BallPrefabs;
    private string[] BallPrefabFileNames = { "Apple", "Bananas", "Cherries", "Kiwi", "Orange", "Strawberry" };
    public GameObject mapController;
    private string mapControllerFileName = "MapController";
    public GameObject ballQueue;
    private string ballQueueFileName = "BallQueue";
    public Transform route;
    private string routeFileName = "Path 2";

    
    public PrefabController()
    {
        BallPrefabs = new GameObject[BallPrefabFileNames.Length];
        for (int i = 0; i < BallPrefabFileNames.Length; i++)
        {
            BallPrefabs[i] = Resources.Load(prefabFolderPath + BallPrefabFileNames[i]) as GameObject;
        }

        mapController = Resources.Load(prefabFolderPath + mapControllerFileName) as GameObject;
        ballQueue = Resources.Load(prefabFolderPath + ballQueueFileName) as GameObject;
        route = (Resources.Load(prefabFolderPath + routeFileName) as GameObject).transform;
        if (route == null) Debug.Log("NAUR");
    }
}
