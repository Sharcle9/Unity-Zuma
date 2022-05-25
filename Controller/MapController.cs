using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    static int ballType = 0;
    public PrefabController prefabController;
    public MapData mapData;
    private bool canSpawn;
    private GameObject[] BallPrefabs;

    public MapController(MapData mapData)
    {
        this.mapData = mapData;
    }

    private void Start()
    {
        prefabController = new();
        BallPrefabs = prefabController.BallPrefabs;

        GameObject playerBall = BallPrefabs[3];
        Instantiate(playerBall, this.transform).AddComponent<PlayerController>();

        GameObject ballQueue = prefabController.ballQueue;
        Instantiate(ballQueue, this.transform).AddComponent<BallQueue>();


    }

    private void Update()
    {

    }

    public BallType GenerateBall()
    {
        return (BallType) ballType;
    }


    public void GenerateBallOnClick()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float x = mousePos.x;
        float y = mousePos.y;
        GameObject ballObject = Instantiate(BallPrefabs[1], this.transform);
        ballObject.transform.position = new Vector3(x, y, 10f);
        ballObject.transform.parent = this.transform;
        ballObject.name = "ball";
    }

    public void GenerateQueue(int count)
    {
        BallQueue ballQueue = gameObject.AddComponent<BallQueue>();
    }

}
