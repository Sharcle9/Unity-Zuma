using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallQueue : MonoBehaviour
{
    public int length = 5; 
    private Transform route;
    private GameObject[] BallPrefabs;

    public float speedMultiplier = 1f;
    private int curvesRemaining;

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

        InitParamSetUp();

        GameObject prevBall = null;
        for (int i = 0; i < length; i++)
        {
            GameObject ball = Instantiate(BallPrefabs[4], this.transform);
            ball.AddComponent<Ball>();
            ball.GetComponent<Ball>().Init(0.5f, speedMultiplier, 0, curvesRemaining, stepSize, tStepSize, route, i == 0, ballRadius);

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

        InitParamSetUp();

        GameObject ball = Instantiate(BallPrefabs[4], this.transform);
        ball.AddComponent<Ball>();
        ball.GetComponent<Ball>().Init(0.99f, speedMultiplier, 1, curvesRemaining, stepSize, tStepSize, route, true, ballRadius);

        GameObject ballAhead = Instantiate(BallPrefabs[3], this.transform);
        ballAhead.AddComponent<Ball>();
        ballAhead.GetComponent<Ball>().Init(0.5f, speedMultiplier, 0, curvesRemaining, stepSize, tStepSize, route, true, ballRadius);
        ballAhead.GetComponent<Ball>().SetLocationRelativeToBall(ball, true);
    }

    public void InitParamSetUp()
    {
        stepSize *= speedMultiplier;
        curvesRemaining = (route.childCount - 1) / 3;
    }
}
