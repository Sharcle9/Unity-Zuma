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
        this.transform.position = Vector2.zero;
        prefabController = new();
        BallPrefabs = prefabController.BallPrefabs;

        GameObject ballQueue = Instantiate(prefabController.ballQueue, this.transform);
        ballQueue.AddComponent<BallQueue>();

        BallType ballType = (BallType) Random.Range(0, 5);
        GameObject playerBall = Instantiate(BallPrefabs[(int) ballType], this.transform);
        playerBall.AddComponent<PlayerController>();
        playerBall.GetComponent<PlayerController>().SetBallQueue(ballQueue.transform);
        playerBall.GetComponent<PlayerController>().ballType = ballType;
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
