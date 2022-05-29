using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public PrefabController prefabController;
    public MapData mapData;
    private GameObject[] BallPrefabs;

    public MapController(MapData mapData)
    {
        this.mapData = mapData;
    }

    private void Start()
    {
        this.transform.position = Vector2.zero;
        prefabController = new();

        GameObject ballQueue = Instantiate(prefabController.ballQueue, this.transform);
        ballQueue.AddComponent<BallQueue>();

        GameObject playerController = Instantiate(prefabController.playerController, this.transform);
        playerController.AddComponent<PlayerController>();
        playerController.GetComponent<PlayerController>().SetBallQueue(ballQueue.transform);
    }

    private void Update()
    {

    }

    public void GenerateQueue(int count)
    {
        BallQueue ballQueue = gameObject.AddComponent<BallQueue>();
    }

}
