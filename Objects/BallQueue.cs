using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallQueue : MonoBehaviour
{
    public int length = 5; 
    private Transform route;
    private GameObject[] BallPrefabs;

    public float speedMultiplier = 1f;

    private float stepSize = 0.005f;
    private float tStepSize = 0.001f;
    private float ballRadius = 0.42f;

    private PrefabController prefabController;

    private void Start()
    {
        InitDebug();
    }

    private void Update()
    {

        
    }


    public void Init()
    {

        prefabController = new PrefabController();
        BallPrefabs = prefabController.BallPrefabs;
        route = prefabController.route;


        GameObject prevBall = null;
        for (int i = 0; i < length; i++)
        {
            GameObject ball = Instantiate(BallPrefabs[4], this.transform);
            ball.AddComponent<Ball>();
            ball.GetComponent<Ball>().Init(0.5f, speedMultiplier, 0, stepSize, tStepSize, route, i == 0, ballRadius);

            ball.GetComponent<Ball>().ahead = prevBall;
            if (ball.GetComponent<Ball>().ahead != null)
            {
                ball.GetComponent<Ball>().ahead.GetComponent<Ball>().behind = ball;
            }
            prevBall = ball;
        }
    }

    public void InitDebug()
    {
        prefabController = new PrefabController();
        BallPrefabs = prefabController.BallPrefabs;
        route = prefabController.route;

        GameObject ball = Instantiate(BallPrefabs[4], this.transform);
        ball.AddComponent<Ball>();
        ball.GetComponent<Ball>().Init(0.5f, speedMultiplier, 0, stepSize, tStepSize, route, true, ballRadius);

        GameObject ballAhead1 = Instantiate(BallPrefabs[5], this.transform);
        ballAhead1.AddComponent<Ball>();
        ballAhead1.GetComponent<Ball>().Init(0.5f, speedMultiplier, 0, stepSize, tStepSize, route, true, ballRadius);
        ballAhead1.GetComponent<Ball>().SetLocationRelativeToBall(ball, true);

        GameObject ballAhead2 = Instantiate(BallPrefabs[2], this.transform);
        ballAhead2.AddComponent<Ball>();
        ballAhead2.GetComponent<Ball>().Init(0.5f, speedMultiplier, 0, stepSize, tStepSize, route, true, ballRadius);
        ballAhead2.GetComponent<Ball>().SetLocationRelativeToBall(ballAhead1, true);

        GameObject ballAhead3 = Instantiate(BallPrefabs[1], this.transform);
        ballAhead3.AddComponent<Ball>();
        ballAhead3.GetComponent<Ball>().Init(0.5f, speedMultiplier, 0, stepSize, tStepSize, route, true, ballRadius);
        ballAhead3.GetComponent<Ball>().SetLocationRelativeToBall(ballAhead2, true);

        GameObject ballAhead4 = Instantiate(BallPrefabs[0], this.transform);
        ballAhead4.AddComponent<Ball>();
        ballAhead4.GetComponent<Ball>().Init(0.5f, speedMultiplier, 0, stepSize, tStepSize, route, true, ballRadius);
        ballAhead4.GetComponent<Ball>().SetLocationRelativeToBall(ballAhead3, true);
    }
}
