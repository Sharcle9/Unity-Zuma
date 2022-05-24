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
        Init();
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
            ball.GetComponent<Ball>().Init(0, speedMultiplier, 0, curvesRemaining, stepSize, tStepSize, route, i == 0, ballRadius);

            ball.GetComponent<Ball>().prev = prevBall;
            if (ball.GetComponent<Ball>().prev != null)
            {
                ball.GetComponent<Ball>().prev.GetComponent<Ball>().next = ball;
            }
            prevBall = ball;
        }
    }

    public void InitParamSetUp()
    {
        stepSize *= speedMultiplier;
        curvesRemaining = (route.childCount - 1) / 3;
    }
}
