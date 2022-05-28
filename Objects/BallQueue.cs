using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallQueue : MonoBehaviour
{
    public int length = 5; 
    private Transform route;
    private GameObject[] BallPrefabs;

    public float speedMultiplier = 1f;

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
            ball.GetComponent<Ball>().Init(0.5f, 0,  route, ballRadius, (BallType) 4);

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

        GameObject ball0 = GenerateBall((BallType) 0);

        GameObject ball1 = GenerateBall((BallType) 1);

        GameObject ball2 = GenerateBall((BallType) 2);

        GameObject ball3 = GenerateBall((BallType) 3);

        GameObject ball4 = GenerateBall((BallType) 4);

        GameObject[] balls = { ball0, ball1, ball2, ball3, ball4 };

        SetRelation(balls);
    }

    private GameObject GenerateBall(BallType ballType)
    {
        GameObject ball = Instantiate(BallPrefabs[(int) ballType], this.transform);
        ball.AddComponent<Ball>();
        ball.GetComponent<Ball>().Init(0.5f, 0, route, ballRadius, ballType);

        return ball;
    }

    private void SetRelation(GameObject[] balls)
    {
        GameObject behind = null;

        for (int i = 0; i < balls.Length; i++)
        {
            if (behind != null) balls[i].GetComponent<Ball>().SetLocationRelativeToBall(behind, true);

            behind = balls[i];
        }
    }
}
