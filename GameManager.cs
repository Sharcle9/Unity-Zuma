using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapConfig mapConfig;
    public GameObject targetBallPrefab;
    public ObjectPool<TargetBall> targetBallPool;

    private List<TargetBall> targetBallSegmentList = new List<TargetBall>();
    public float moveSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        targetBallPool = new ObjectPool<TargetBall>(InstanceTargetBallFunc, 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private TargetBall InstanceTargetBallFunc()
    {
        GameObject targetBall = Instantiate(targetBallPrefab, transform.Find("TargetBalls"));
        targetBall.SetActive(false);
        TargetBall targetBallClass = targetBall.AddComponent<TargetBall>();
        return targetBallClass;
    }

    private void FirstBallMove()
    {
        if (targetBallSegmentList.Count == 0)
        {
            TargetBall targetBall = targetBallPool.GetObject().Init(this);
            targetBallSegmentList.Add(targetBall);
            return;
        }

        TargetBall firstBall = targetBallSegmentList[0];

        if (firstBall.IsNotStartBall())
        {
            TargetBall ball = targetBallPool.GetObject().Init(this);
            ball.position = 0;

            ball.NextBall = firstBall;
            firstBall.PrevBall = ball;

            targetBallSegmentList[0] = ball;
            firstBall = ball;
        }

        firstBall.position += Time.deltaTime * moveSpeed;

        while (firstBall.NextBall != null)
        {
            if (firstBall.NextBall.position < firstBall.position + 1)
            {
                firstBall.NextBall.position = firstBall.position + 1;
            }
            firstBall = firstBall.NextBall;
        }

    }
}
