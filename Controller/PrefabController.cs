using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabController
{
    private string prefabFolderPath = "Prefabs/";
    public GameObject[] BallPrefabs;
    private string[] BallPrefabFileNames = { "Apple", "Bananas", "Cherries", "Kiwi", "Orange", "Strawberry" };
    public GameObject MapController;
    private string mapControllerFileName = "MapController";
    public Transform route;
    private string routeFileName = "Path";

    
    public PrefabController()
    {
        BallPrefabs = new GameObject[BallPrefabFileNames.Length];
        for (int i = 0; i < BallPrefabFileNames.Length; i++)
        {
            BallPrefabs[i] = Resources.Load(prefabFolderPath + BallPrefabFileNames[i]) as GameObject;
        }
        MapController = Resources.Load(prefabFolderPath + mapControllerFileName) as GameObject;
        route = (Resources.Load(prefabFolderPath + routeFileName) as GameObject).transform;
        if (route == null) Debug.Log("NAUR");
    }
}
