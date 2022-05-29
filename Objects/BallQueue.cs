using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BallQueue : MonoBehaviour
{
    public int length = 5; 
    private Transform route;
    private Vector2 spawnPoint;
    private GameObject[] BallPrefabs;

    public float speedMultiplier = 1f;

    private float ballRadius = 0.42f;

    private PrefabController prefabController;

    private int segmentLength;
    private BallType ballType = 0;
    private float radius = 0.42f;
    private GameObject ahead = null;
    private int totalBallCount = 0;


    private void Start()
    {
        Init();
    }

    private void Update()
    {
        // if finished spawning or the ball ahead is too close
        if (totalBallCount <= 0 || (ahead != null &&
            radius + ahead.GetComponent<Ball>().ballRadius > Vector2.Distance(spawnPoint, ahead.transform.position)))
        {
            return;
        }

        GameObject ballObject = GenerateBall((BallType)ballType);
        SetRelation(ballObject, ahead);
        ahead = ballObject;
        segmentLength--;
        totalBallCount--;
            
        if (segmentLength <= 0)
        {
            segmentLength = Random.Range(1, 3);
            ballType = (BallType)Random.Range(0, 5);
        }
        
    }

    public void Init()
    {
        prefabController = new PrefabController();
        BallPrefabs = prefabController.BallPrefabs;
        route = prefabController.route;
        spawnPoint = Ball.GetBezierPoint(0, route, 0);
        totalBallCount = 30;
        List<GameObject> balls = new();
    }

    private GameObject GenerateBall(BallType ballType)
    {
        GameObject ball = Instantiate(BallPrefabs[(int) ballType], this.transform);
        ball.AddComponent<Ball>();
        ball.GetComponent<Ball>().Init(0f, 0, route, ballRadius, ballType);

        return ball;
    }

    private void SetRelation(List<GameObject> balls)
    {
        GameObject behind = null;

        for (int i = 0; i < balls.Count; i++)
        {
            if (behind != null) balls[i].GetComponent<Ball>().SetLocationRelativeToBall(behind, true);

            behind = balls[i];
        }
    }

    private static void SetRelation(GameObject behind, GameObject ahead)
    {
        if (ahead != null) ahead.GetComponent<Ball>().behind = behind;
        if (behind != null) behind.GetComponent<Ball>().ahead = ahead;
    }
}
