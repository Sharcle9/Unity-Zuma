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

    private float stepSize = 0.005f;
    private float errorSize;
    private float tStepSize = 0.001f;
    private float ballRadius = 0.42f;

    private Vector2 mousePos;
    private Vector2 playerPos;
    private bool isShooting = false;

    private Transform ballQueue;
    private bool hasStarted = false;
    private bool isInQueue = true;
    private Transform collidingBall;
    private bool counterclockwise = true;

    private void Update()
    {

        if (ahead == null) isHead = true;

        if (isShooting)
        {
            collidingBall = IsTouchingQueue();
            if (collidingBall != null)
            {
                isShooting = false;
                SwitchParent(ballQueue);
                JoinQueue(collidingBall);
                route = collidingBall.GetComponent<Ball>().route;
                curvesRemaining = (route.childCount - 1) / 3;
                totalCurves = (route.childCount - 1) / 3;
            } 
            else if (IsVisible())
            {
                Shoot();
            }
            
        } 
        else if (!isInQueue)
        {
            if (!IsCloseToTrack())
            {
                // Debug.Log(Vector2.Distance(transform.position, collidingBall.position));
                RotateAroundBall(collidingBall, counterclockwise);
            }
        } 
        /*
        else if (isHead) Move();
        else FollowDebug();
        */
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
        this.isShooting = isFromPlayer;
        this.ballQueue = ballQueue;
        this.ballRadius = ballRadius;
        this.isInQueue = false;
        errorSize = stepSize / 50;
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

    private void FollowDebug()
    {
        if (ahead == null) return;

        Vector2 prevBallPos = ahead.transform.position;


        if (!hasStarted && Vector2.Distance(prevBallPos, transform.position) < 2 * ballRadius) return;

        Move();
    }

    /*
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
    */
    
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
                BounceBack(pointerBallPos, pointerBallRadius, tStepSize, t);
                return pointerBall;
            }
        }

        return null;
    }

    private void BounceBack(Vector2 collidingBallPos, float collidingBallRadius, float tStepSize, float currentT)
    {
        float currentDistance = Vector2.Distance(collidingBallPos, this.transform.position);

        while (currentDistance < this.ballRadius + collidingBallRadius)
        {
            tStepSize *= 2;
            currentDistance = Vector2.Distance(collidingBallPos, GetLinearPos(currentT - tStepSize, playerPos, mousePos, shootSpeedMultiplier));
        }

        float deltaT = 0.5f * tStepSize;
        // binary search
        while (currentDistance < this.ballRadius + collidingBallRadius - errorSize || currentDistance > this.ballRadius + collidingBallRadius + errorSize)
        {
            if (currentDistance < this.ballRadius + collidingBallRadius) tStepSize += deltaT;
            else tStepSize -= deltaT;

            currentDistance = Vector2.Distance(collidingBallPos, GetLinearPos(currentT - tStepSize, playerPos, mousePos, shootSpeedMultiplier));
            deltaT *= 0.5f;
        }
        this.transform.position = GetLinearPos(currentT - tStepSize, playerPos, mousePos, shootSpeedMultiplier);
    }

    private void SwitchParent(Transform parent)
    {
        this.transform.parent = parent;
    }

    /* Convert a player-launched ball into a ball on track and join a queue */
    private void JoinQueue(Transform collidingBall)
    {
        if (IsAheadOfCollidingBall(collidingBall))
        {
            Debug.Log("Ahead");
            SetRotationDirection(true);

            SetRelation(collidingBall.gameObject, true);
            GameObject ballAhead = collidingBall.GetComponent<Ball>().ahead;
            SetRelation(ballAhead, false);

        }
        else
        {
            Debug.Log("Behind");
            SetRotationDirection(false);

            SetRelation(collidingBall.gameObject, false);
            GameObject ballAhead = collidingBall.GetComponent<Ball>().behind;
            SetRelation(ballAhead, true);

        }

        
        /*
        float nextT = collidingBall.GetComponent<Ball>().t;
        float nextSpeedMultiplier = collidingBall.GetComponent<Ball>().speedMultiplier;
        int nextCurrentCurveIndex = collidingBall.GetComponent<Ball>().currentCurveIndex;
        int nextCurvesRemaining = collidingBall.GetComponent<Ball>().curvesRemaining;
        float nextStepSize = collidingBall.GetComponent<Ball>().stepSize;
        float nextTStepSize = collidingBall.GetComponent<Ball>().tStepSize;
        Transform nextRoute = collidingBall.GetComponent<Ball>().route;
        Init(nextT, nextSpeedMultiplier, nextCurrentCurveIndex, nextCurvesRemaining, nextStepSize, nextTStepSize, nextRoute, ahead == null, ballRadius);
        */
    }

    // Rotate the current ball around the colliding ball
    // Return true until the ball hits the track
    private void RotateAroundBall(Transform collidingBall, bool counterclockwise)
    {
        float d = Time.deltaTime * shootSpeedMultiplier * 500;

        if (!counterclockwise) d *= -1;

        this.transform.position = RotateAround(collidingBall.position, this.transform.position, d);
    }

    private bool IsCloseToTrack()
    {
        bool isAhead = IsAheadOfCollidingBall(this.collidingBall);
        Ball ball = this.collidingBall.GetComponent<Ball>();
        Location location = GetLocationRelativeToBall(ball.transform.position, 
            ball.t, 
            this.ballRadius, 
            ball.ballRadius, 
            this.tStepSize, 
            this.errorSize,
            this.route, 
            ball.currentCurveIndex, 
            isAhead);

        float distance = Vector2.Distance(GetBezierPoint(location.t, this.route, location.currentCurveIndex), this.transform.position);
        Debug.Log(distance);
        return distance < shootSpeedMultiplier * 0.02;
    }

    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    private static Vector2 RotateAround(Vector2 center, Vector2 v, float degrees)
    {
        return Rotate(v - center, degrees) + center;
    }

    private void SetRotationDirection(bool toAheadOfTargetBall)
    {
        GameObject relativeBallObject = toAheadOfTargetBall ? this.collidingBall.GetComponent<Ball>().ahead : this.collidingBall.GetComponent<Ball>().behind;
        Vector2 relativeBallPos;
        Ball ball = this.collidingBall.GetComponent<Ball>();

        if (relativeBallObject != null)
        {
            relativeBallPos = relativeBallObject.transform.position;
        }
        else
        {
            relativeBallPos = GetBezierPoint(toAheadOfTargetBall ? ball.t + 0.01f : ball.t - 0.01f, ball.route, ball.currentCurveIndex);
        }

        float angle = Vector2.SignedAngle((Vector2)this.transform.position - (Vector2)this.collidingBall.position,
            relativeBallPos - (Vector2)this.collidingBall.position);
        Debug.Log((Vector2)this.transform.position);
        Debug.Log(relativeBallPos);

        // if going counterclockwise reaches track faster, set counterclockwise to true
        counterclockwise = angle > 0;
    }

    private bool IsAheadOfCollidingBall(Transform collidingBall)
    {
        Ball ball = collidingBall.GetComponent<Ball>();

        GameObject ballAhead = ball.ahead;
        GameObject ballBehind = ball.behind;

        Vector2 collidingBallPos = collidingBall.position;

        // if the colliding ball is by itself
        if (ballAhead == null && ballBehind == null)
        {
            Vector2 imaginaryBallAheadPos = GetBezierPoint(ball.t + 0.01f, ball.route, ball.currentCurveIndex);

            float angle = Vector2.Angle((Vector2) this.transform.position - collidingBallPos, 
                imaginaryBallAheadPos - collidingBallPos);

            if (angle < 90f) return true;
            else return false;
        }

        // if the colliding ball is the head
        if (ballAhead == null)
        {
            float angle = Vector2.Angle((Vector2)this.transform.position - collidingBallPos, 
                (Vector2) ballBehind.transform.position - collidingBallPos);

            if (angle < 90f) return false;
            else return true;
        }

        // if the colliding ball is the tail
        if (ballBehind == null)
        {
            float angle = Vector2.Angle((Vector2)this.transform.position - collidingBallPos, 
                (Vector2)ballAhead.transform.position - collidingBallPos);

            if (angle < 90f) return true;
            else return false;
        }

        float angleToAhead = Vector2.Angle((Vector2)this.transform.position - collidingBallPos, 
            (Vector2)ballAhead.transform.position - collidingBallPos);
        float angleToBehind = Vector2.Angle((Vector2)this.transform.position - collidingBallPos, 
            (Vector2)ballBehind.transform.position - collidingBallPos);

        if (angleToAhead < angleToBehind) return true;
        else return false;
    }

    /// <summary>
    /// Get the location behind/ahead of a given ball that tightly touches it
    /// </summary>
    /// <param name="targetBallPos">position of the target ball</param>
    /// <param name="targetBallT">t value of the target ball on its current Bezier curve</param>
    /// <param name="radius">radius of the ball being added</param>
    /// <param name="targetBallRadius">radius of the target ball</param>
    /// <param name="tStepSize">how much t steps initially</param>
    /// <param name="errorSize">the amount of error allowed in binary search</param>
    /// <param name="route">current route</param>
    /// <param name="targetBallCurveIndex">curve index of the target ball on its current Bezier curve</param>
    /// <param name="aheadOfGivenBall">whether to get the position ahead of the target ball</param>
    /// <returns></returns>
    private static Location GetLocationRelativeToBall(Vector2 targetBallPos, float targetBallT, float radius, 
        float targetBallRadius, float tStepSize, float errorSize, Transform route, int targetBallCurveIndex, bool aheadOfGivenBall)
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

    /* Set the location ahead/behind of a given ball */
    public void SetLocationRelativeToBall(GameObject targetBallObject, bool aheadOfGivenBall)
    {
        Ball targetBall = targetBallObject.GetComponent<Ball>();
        float targetBallT = targetBall.t;
        int targetBallCurveIndex = targetBall.currentCurveIndex;
        float targetBallRadius = targetBall.ballRadius;
        Vector2 targetBallPos = GetBezierPoint(targetBallT, route, targetBallCurveIndex);
        Location location = GetLocationRelativeToBall(targetBallPos, targetBallT, ballRadius, targetBallRadius, tStepSize,
            errorSize, route, targetBallCurveIndex, aheadOfGivenBall);

        SetLocation(location);
        SetRelation(targetBallObject, aheadOfGivenBall);
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

    // Set the ahead/behind relation between the target ball and current ball
    private void SetRelation(GameObject targetBallObject, bool IsAheadOfTargetBall)
    {
        if (IsAheadOfTargetBall)
        {
            this.behind = targetBallObject;
            if (targetBallObject != null) targetBallObject.GetComponent<Ball>().ahead = this.gameObject;
        }
        else
        {
            this.ahead = targetBallObject;
            if (targetBallObject != null) targetBallObject.GetComponent<Ball>().behind = this.gameObject;
        }
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
