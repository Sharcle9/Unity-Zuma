using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject prefab;
    public bool facingPlayer = false;
    public bool isHead;
    public BallType ballType = 0;
    public GameObject ahead = null;
    public GameObject behind = null;
    private Transform route;
    private float t;
    public float speedMultiplier;
    public float shootSpeedMultiplier = 1f;
    private int currentCurveIndex = 0;
    private int curvesRemaining;
    private int totalCurves;

    private float stepSize;
    private float errorSize;
    private float tStepSize;
    private float ballRadius;

    private Vector2 mousePos;
    private Vector2 playerPos;
    private bool isFromPlayer = false;

    private Transform ballQueue;
    private bool hasStarted = false;

    private void Update()
    {

        if (isFromPlayer)
        {
            Transform collidingBall = IsTouchingQueue();
            if (collidingBall != null)
            {
                isFromPlayer = false;
                SwitchParent(ballQueue);
                JoinQueue(collidingBall);
            }
            
        }

        if (isFromPlayer && IsVisible())
        {
            Shoot();
        }
        else if (isHead) /*Move()*/;
        else FollowDebug();

    }

    public void Init(float t, float speedMultiplier, int currentCurveIndex, int curvesRemaining, float stepSize, float tStepSize, Transform route, bool isHead, float ballRadius)
    {
        this.t = t;
        this.speedMultiplier = speedMultiplier;
        this.currentCurveIndex = currentCurveIndex;
        this.curvesRemaining = curvesRemaining;
        this.stepSize = stepSize;
        this.tStepSize = tStepSize;
        this.route = route;
        this.isHead = isHead;
        this.ballRadius = ballRadius;
        stepSize *= speedMultiplier;
        errorSize = stepSize / 50;
        curvesRemaining = (route.childCount - 1) / 3;
        totalCurves = (route.childCount - 1) / 3;
        transform.position = GetBezierPoint(t, route, currentCurveIndex);
    }

    public void Init(Vector2 playerPos, Vector2 mousePos, bool isFromPlayer, Transform ballQueue, float ballRadius)
    {
        t = 0;
        this.playerPos = playerPos;
        this.mousePos = mousePos;
        this.isFromPlayer = isFromPlayer;
        this.ballQueue = ballQueue;
        this.ballRadius = ballRadius;
    }

    public void Shoot()
    {
        this.transform.position = GetLinearPos(t, playerPos, mousePos, shootSpeedMultiplier);
        t += Time.deltaTime;
    }

    public Vector2 GetLinearPos(float t, Vector2 start, Vector2 end, float speed)
    {
        float distance = Vector2.Distance(start, end);

        float timeToReachEnd = distance / speed;

        return new Vector2(start.x + (end.x - start.x) * speed * t * 15f / distance, start.y + (end.y - start.y) * speed * t * 15f / distance);
    }

    public void Move()
    {
        if (curvesRemaining == 0) return;

        t = GetNextT(t, tStepSize, stepSize, errorSize, route, currentCurveIndex);

        transform.position = GetBezierPoint(t, route, currentCurveIndex);
        hasStarted = true;

        if (t >= 1)
        {
            t = 0;
            // Debug.Log("t = " + t);
            currentCurveIndex++;
            curvesRemaining--;
        }

    }

    private void Follow()
    {
        if (ahead == null) return;

        Vector2 prevBallPos = ahead.transform.position;

        
        if (Vector2.Distance(prevBallPos, transform.position) < 2 * ballRadius) return;
        t = GetNextTFollow(t, prevBallPos, ballRadius, tStepSize, errorSize, route, currentCurveIndex);

        transform.position = GetBezierPoint(t, route, currentCurveIndex);

        if (t >= 1)
        {
            t = 0;
            // Debug.Log("t = " + t);
            currentCurveIndex++;
            curvesRemaining--;
        }
    }

    private void FollowDebug()
    {
        if (ahead == null) return;

        Vector2 prevBallPos = ahead.transform.position;


        if (!hasStarted && Vector2.Distance(prevBallPos, transform.position) < 2 * ballRadius) return;

        Move();
    }

    private static float GetNextTFollow(float currentT, Vector2 prevBallPos, float radius, float tStepSize, float errorSize, Transform route, int currentCurveIndex)
    {
        float currentDistance = Vector2.Distance(prevBallPos, GetBezierPoint(currentT + tStepSize, route, currentCurveIndex));
        int count = 0;

        while (currentDistance > 2 * radius)
        {
            tStepSize *= 2;
            currentDistance = Vector2.Distance(prevBallPos, GetBezierPoint(currentT + tStepSize, route, currentCurveIndex));
            count++;
        }

        float deltaT = 0.5f * tStepSize;
        // binary search
        while (currentDistance < 2 * radius - errorSize || currentDistance > 2 * radius + errorSize)
        {
            if (currentDistance < 2 * radius) tStepSize -= deltaT;
            else tStepSize += deltaT;

            currentDistance = Vector2.Distance(prevBallPos, GetBezierPoint(currentT + tStepSize, route, currentCurveIndex));
            deltaT *= 0.5f;
            count++;
        }

        // Debug.Log(count);

        return currentT + tStepSize;
    }

    private static Location GetLocationRelativeToBall(Vector2 targetBallPos, float targetBallT, float radius, float targetBallRadius, float tStepSize, float errorSize, Transform route, int targetBallCurveIndex, bool aheadOfGivenBall)
    {

        float currentDistance = Vector2.Distance(targetBallPos, GetBezierPoint(aheadOfGivenBall ? targetBallT + tStepSize : targetBallT - tStepSize, route, targetBallCurveIndex));
        int count = 0;

        while (currentDistance < radius + targetBallRadius)
        {
            tStepSize *= 2;
            currentDistance = Vector2.Distance(targetBallPos, GetBezierPoint(aheadOfGivenBall ? targetBallT + tStepSize : targetBallT - tStepSize, route, targetBallCurveIndex));
            count++;
        }

        float deltaT = 0.5f * tStepSize;
        // binary search
        while (currentDistance < radius + targetBallRadius - errorSize || currentDistance > radius + targetBallRadius + errorSize)
        {
            if (currentDistance < radius + targetBallRadius) tStepSize += deltaT;
            else tStepSize -= deltaT;

            currentDistance = Vector2.Distance(targetBallPos, GetBezierPoint(aheadOfGivenBall ? targetBallT + tStepSize : targetBallT - tStepSize, route, targetBallCurveIndex));
            deltaT *= 0.5f;
            count++;
        }

        if (aheadOfGivenBall)
        {
            if (targetBallT + tStepSize > 1 && targetBallCurveIndex <= GetNumberOfCurves(route) - 1)
            {
                return GetLocationRelativeToBall(targetBallPos, 0, radius, targetBallRadius, tStepSize, errorSize, route, targetBallCurveIndex + 1, aheadOfGivenBall);
            }
            else
            {
                return new Location(targetBallT + tStepSize, targetBallCurveIndex);
            }
        }
        else
        {
            if (targetBallT - tStepSize < 0 && targetBallCurveIndex > 0)
            {
                return GetLocationRelativeToBall(targetBallPos, 1, radius, targetBallRadius, tStepSize, errorSize, route, targetBallCurveIndex - 1, aheadOfGivenBall);
            }
            else
            {
                return new Location(targetBallT - tStepSize, targetBallCurveIndex);
            }
        }
    }

    /* Get the next t value when moving at a constant speed */
    private float GetNextT(float currentT, float tStepSize, float stepSize, float errorSize, Transform route, int currentCurveIndex)
    {

        Vector2 currentPoint = GetBezierPoint(currentT, route, currentCurveIndex);
        float currentDistance = Vector2.Distance(currentPoint, GetBezierPoint(currentT + tStepSize, route, currentCurveIndex));
        int count = 0;

        while (currentDistance < stepSize)
        {
            tStepSize *= 2;
            currentDistance = Vector2.Distance(currentPoint, GetBezierPoint(currentT + tStepSize, route, currentCurveIndex));
            count++;
        }

        float deltaT = 0.5f * tStepSize;
        // binary search
        while (currentDistance < stepSize - errorSize || currentDistance > stepSize + errorSize)
        {
            if (currentDistance < stepSize) tStepSize += deltaT;
            else tStepSize -= deltaT;

            currentDistance = Vector2.Distance(currentPoint, GetBezierPoint(currentT + tStepSize, route, currentCurveIndex));
            deltaT *= 0.5f;
            count++;
        }

        // Debug.Log(count);

        return currentT + tStepSize;
    }

    private static Vector2 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return p0 * Mathf.Pow((1 - t), 3) + (3 * Mathf.Pow((1 - t), 2) * t * p1) + ((1 - t) * 3 * Mathf.Pow(t, 2) * p2) + p3 * Mathf.Pow(t, 3);
    }

    private static Vector2 GetBezierPoint(float t, Transform route)
    {
        return GetBezierPoint(t, route.GetChild(0).position, route.GetChild(1).position, route.GetChild(2).position, route.GetChild(3).position);
    }

    private static Vector2 GetBezierPoint(float t, Transform route, int currentCurveIndex)
    {
        // Debug.Log(currentCurveIndex);
        int vertex0 = currentCurveIndex * 3;
        int vertex1 = currentCurveIndex * 3 + 1;
        int vertex2 = currentCurveIndex * 3 + 2;
        int vertex3 = currentCurveIndex * 3 + 3;
        return GetBezierPoint(t, route.GetChild(vertex0).position, route.GetChild(vertex1).position, route.GetChild(vertex2).position, route.GetChild(vertex3).position);
    }

    /* Checks if the ball is off-screen*/
    private bool IsVisible()
    {
        if (!this.GetComponent<SpriteRenderer>().isVisible)
        {
            Destroy(this.gameObject);
            return false;
        }
        else return true;
    }

    private Transform IsTouchingQueue()
    {
        int currentBallCount = ballQueue.childCount;
        Vector2[] currentBallsPos = new Vector2[currentBallCount];

        for (int i = 0; i < currentBallCount; i++)
        {
            Transform pointerBall = ballQueue.GetChild(i);
            Vector2 pointerBallPos = pointerBall.position;
            float pointerBallRadius = pointerBall.GetComponent<Ball>().ballRadius;
            if (Vector2.Distance(pointerBallPos, this.transform.position) < ballRadius + pointerBallRadius)
            {
                return pointerBall;
            }
        }

        return null;
    }

    private void SwitchParent(Transform parent)
    {
        this.transform.parent = parent;
    }

    /* Convert a player-launched ball into a ball on track and join a queue */
    private void JoinQueue(Transform collidingBall)
    {
        GameObject prevBall = collidingBall.GetComponent<Ball>().behind;

        if (prevBall != null) prevBall.GetComponent<Ball>().behind = this.gameObject;
        collidingBall.GetComponent<Ball>().ahead = this.gameObject;

        this.ahead = prevBall;
        this.behind = collidingBall.gameObject;
        
        float nextT = collidingBall.GetComponent<Ball>().t;
        float nextSpeedMultiplier = collidingBall.GetComponent<Ball>().speedMultiplier;
        int nextCurrentCurveIndex = collidingBall.GetComponent<Ball>().currentCurveIndex;
        int nextCurvesRemaining = collidingBall.GetComponent<Ball>().curvesRemaining;
        float nextStepSize = collidingBall.GetComponent<Ball>().stepSize;
        float nextTStepSize = collidingBall.GetComponent<Ball>().tStepSize;
        Transform nextRoute = collidingBall.GetComponent<Ball>().route;
        Init(nextT, nextSpeedMultiplier, nextCurrentCurveIndex, nextCurvesRemaining, nextStepSize, nextTStepSize, nextRoute, ahead == null, ballRadius);
    }

    /* Get a suitable location on track ahead of a given ball */
    public void SetLocationRelativeToBall(float targetBallT, int targetBallCurveIndex, float targetBallRadius, bool aheadOfGivenBall)
    {
        Vector2 nextBallPos = GetBezierPoint(targetBallT, route, targetBallCurveIndex);
        Location location = GetLocationRelativeToBall(nextBallPos, targetBallT, ballRadius, targetBallRadius, tStepSize, errorSize, route, targetBallCurveIndex, aheadOfGivenBall);

        SetLocation(location);
    }

    public void SetLocationRelativeToBall(Location location, float targetBallRadius, bool aheadOfGivenBall)
    {
        SetLocationRelativeToBall(location.t, location.currentCurveIndex, targetBallRadius, aheadOfGivenBall);
    }

    public void SetLocationRelativeToBall(Ball targetBall, bool aheadOfGivenBall)
    {
        SetLocationRelativeToBall(targetBall.GetLocation(), targetBall.ballRadius, aheadOfGivenBall);
    }

    public void SetLocationRelativeToBall(GameObject targetBall, bool aheadOfGivenBall)
    {
        SetLocationRelativeToBall(targetBall.GetComponent<Ball>(), aheadOfGivenBall);
    }


    private Location GetLocation()
    {
        return new Location(t, currentCurveIndex);
    }

    private void SetLocation(Location location)
    {
        this.transform.position = GetBezierPoint(location.t, route, location.currentCurveIndex);
        this.t = location.t;
        this.currentCurveIndex = location.currentCurveIndex;
    }

    private static int GetNumberOfCurves(Transform route)
    {
        return (route.childCount - 1) / 3;
    }
}

public class Location
{
    public float t;
    public int currentCurveIndex;

    public Location(float t, int currentCurveIndex)
    {
        this.t = t;
        this.currentCurveIndex = currentCurveIndex;
    }
}
