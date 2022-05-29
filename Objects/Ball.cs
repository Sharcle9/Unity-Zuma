using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject prefab;
    public bool isHead = false;
    public bool isTail = false;
    public BallType ballType = 0;
    public GameObject ahead = null;
    public GameObject behind = null;
    public bool hasStarted = false;
    private Transform route;
    private float t;
    public float speedMultiplier = 0.8f;
    public float idleSpeedMultiplier = 0.3f;
    public float shootSpeedMultiplier = 1f;
    private int currentCurveIndex = 0;
    private int totalCurves;

    private float stepSize = 0.005f;
    private float errorSize;
    private float tStepSize = 0.001f;

    public float ballRadius = 0.42f;

    private Vector2 mousePos;
    private Vector2 playerPos;
    private bool isShooting = false;

    private Transform ballQueue;
    public bool isInQueue = true;
    private Transform collidingBall;
    private bool isAheadOfCollidingBall = false;
    private bool counterclockwise = true;

    private void Update()
    {
        // if (!hasStarted) UpdateHasStarted();
        // if the ball is shooting from player
        if (isShooting) UpdateShootingBall();
        else
        {
            UpdateDestroy();
            SetHeadTailStatus();

            // if the ball is touching the queue and rotating into the queue
            if (!isInQueue) UpdateRotatingBall();
            else
            {
                Push();
                Move(idleSpeedMultiplier);
            }

            if (isTail) Move(speedMultiplier);
        }
    }

    private void UpdateHasStarted()
    {
        if (this.t != 0 || this.currentCurveIndex != 0)
        {
            this.hasStarted = true;
        }
        else if (Vector2.Distance(this.transform.position, this.ahead.transform.position) >= this.ballRadius + this.ahead.GetComponent<Ball>().ballRadius)
        {
            this.hasStarted = true;
        }
    }

    private void UpdateDestroy()
    {
        if (this.t >= 1 && this.currentCurveIndex == this.totalCurves - 1)
        {
            Destroy(this.gameObject);
        }
    }

    private void UpdateShootingBall()
    {
        collidingBall = IsTouchingQueue();
        if (collidingBall != null)
        {
            isShooting = false;
            SwitchParent(ballQueue);
            JoinQueue(collidingBall);
            route = collidingBall.GetComponent<Ball>().route;
            totalCurves = GetNumberOfCurves(route);
        }
        else if (IsVisible())
        {
            Shoot();
        }
    }

    private void UpdateRotatingBall()
    {
        if (!IsCloseToTrack(GetCurrentAngle()))
        {

            if (!isAheadOfCollidingBall && this.behind != null && AreColliding(this.behind.transform, this.transform))
            {
                collidingBall = behind.transform;
                isAheadOfCollidingBall = true;
                counterclockwise = !counterclockwise;
            }


            // Debug.Log(collidingBall.GetComponent<Ball>().ballType + " " + collidingBall.position);
            // Debug.Log((this.transform.position - this.collidingBall.position).magnitude);
            RotateAroundBall(collidingBall, counterclockwise);
        }

        if (IsCloseToTrack(GetCurrentAngle()))
        {
            isInQueue = true;
            Ball ball = collidingBall.GetComponent<Ball>();
            Location location = GetLocationRelativeToBall(ball.transform.position,
                ball.t,
                this.ballRadius,
                ball.ballRadius,
                this.tStepSize,
                this.errorSize,
                this.route,
                ball.currentCurveIndex,
                isAheadOfCollidingBall);
            SetLocation(location);

            ClearCheck();
        }
    }

    private void OnDrawGizmos()
    {
        if (!isInQueue && !isShooting)
        {
            Gizmos.DrawLine(this.transform.position, this.collidingBall.position);
        }
    }

    public void Init(float t, int currentCurveIndex, Transform route, float ballRadius, BallType ballType)
    {
        this.t = t;
        this.currentCurveIndex = currentCurveIndex;
        this.route = route;
        this.ballRadius = ballRadius;
        this.ballType = ballType;
        stepSize *= speedMultiplier;
        errorSize = stepSize / 50;
        transform.position = GetBezierPoint(t, route, currentCurveIndex);
        totalCurves = GetNumberOfCurves(route);

        this.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    public void Init(Vector2 playerPos, Vector2 mousePos, Transform ballQueue, float ballRadius, BallType ballType)
    {
        t = 0;
        this.playerPos = playerPos;
        this.mousePos = mousePos;
        this.isShooting = true;
        this.ballQueue = ballQueue;
        this.ballRadius = ballRadius;
        this.isInQueue = false;
        this.ballType = ballType;
        errorSize = stepSize / 50;
        this.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    public void Shoot()
    {
        this.transform.position = GetLinearPos(t, playerPos, mousePos, shootSpeedMultiplier);
        t += Time.deltaTime;
    }

    public static Vector2 GetLinearPos(float t, Vector2 start, Vector2 end, float speed)
    {
        float distance = Vector2.Distance(start, end);

        float timeToReachEnd = distance / speed;

        return new Vector2(start.x + (end.x - start.x) * speed * t * 15f / distance, start.y + (end.y - start.y) * speed * t * 15f / distance);
    }

    public void Move(float speedMultiplier)
    {
        if (totalCurves - currentCurveIndex == 0) return;

        t = GetNextT(t, tStepSize, stepSize * speedMultiplier, errorSize, route, currentCurveIndex);

        transform.position = GetBezierPoint(t, route, currentCurveIndex);

        if (t >= 1 && currentCurveIndex < totalCurves - 1)
        {
            t = 0;
            currentCurveIndex++;
        }

    }

    /* Get the next t value when moving at a constant speed */
    private static float GetNextT(float currentT, float tStepSize, float stepSize, float errorSize, Transform route, int currentCurveIndex)
    {
        errorSize /= 5;
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

    public static Vector2 GetBezierPoint(float t, Transform route, int currentCurveIndex)
    {
        // Debug.Log(currentCurveIndex);
        int vertex0 = currentCurveIndex * 3;
        int vertex1 = currentCurveIndex * 3 + 1;
        int vertex2 = currentCurveIndex * 3 + 2;
        int vertex3 = currentCurveIndex * 3 + 3;
        return GetBezierPoint(t, route.GetChild(vertex0).position, route.GetChild(vertex1).position, route.GetChild(vertex2).position, route.GetChild(vertex3).position);
    }

    private static bool AreColliding(Transform ball1, Transform ball2)
    {
        return Vector2.Distance(ball1.position, ball2.position) 
            < ball1.GetComponent<Ball>().ballRadius + ball2.GetComponent<Ball>().ballRadius;
    }

    private void Push()
    {
        if (this.behind == null) return;

        Vector2 ballBehindPos = this.behind.transform.position;
        float distance = Vector2.Distance(ballBehindPos, this.transform.position);

        if (distance < this.ballRadius + this.behind.GetComponent<Ball>().ballRadius)
        {
            Location location = GetLocationRelativeToBall(ballBehindPos,
                this.t,
                this.ballRadius,
                this.behind.GetComponent<Ball>().ballRadius,
                this.tStepSize,
                this.errorSize,
                this.route,
                this.currentCurveIndex,
                true);

            SetLocation(location);
        }
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

    private void SetHeadTailStatus()
    {
        if (!isInQueue)
        {
            this.isHead = false;
            this.isTail = false;
            return;
        }
        if (this.behind != null)
        {
            Ball behind = this.behind.GetComponent<Ball>();

            this.isTail = (behind.behind == null && !behind.isInQueue);
        } 
        else
        {
            this.isTail = true;
        }

        if (this.ahead != null) 
        {
            Ball ahead = this.ahead.GetComponent<Ball>();

            this.isHead = (ahead.ahead == null && !ahead.isInQueue);
        }
        else
        {
            this.isHead = true;
        }
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
            if (AreColliding(pointerBall, this.transform))
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
        isAheadOfCollidingBall = IsAheadOfCollidingBall(collidingBall);
        if (isAheadOfCollidingBall)
        {
            GameObject ballAhead = collidingBall.GetComponent<Ball>().ahead;
            SetRelation(collidingBall.gameObject, true);
            SetRelation(ballAhead, false);

            SetRotationDirection();
        }
        else
        {
            GameObject ballAhead = collidingBall.GetComponent<Ball>().behind;
            SetRelation(collidingBall.gameObject, false);
            SetRelation(ballAhead, true);


            SetRotationDirection();
        }

        this.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    // Rotate the current ball around the colliding ball
    // Return true until the ball hits the track
    private void RotateAroundBall(Transform collidingBall, bool counterclockwise)
    {
        float d = Time.deltaTime * shootSpeedMultiplier * 500;

        if (!counterclockwise) d *= -1;

        this.transform.position = RotateAround(collidingBall.position, this.transform.position, d, this.ballRadius + this.collidingBall.GetComponent<Ball>().ballRadius);
    }


    private float GetCurrentAngle()
    {
        GameObject relativeBallObject = this.isAheadOfCollidingBall ? this.ahead : this.behind;
        Vector2 relativeBallPos;
        Ball ball = this.collidingBall.GetComponent<Ball>();

        if (relativeBallObject != null)
        {
            relativeBallPos = relativeBallObject.transform.position;
        }
        else
        {
            relativeBallPos = GetBezierPoint(this.isAheadOfCollidingBall ? ball.t + 0.01f : ball.t - 0.01f, ball.route, ball.currentCurveIndex);
        }

        float angle = Vector2.SignedAngle((Vector2)this.transform.position - (Vector2)this.collidingBall.position,
            relativeBallPos - (Vector2)this.collidingBall.position);

        return angle;
    }

    private bool IsCloseToTrack(float targetAngle)
    {
        if (counterclockwise)
        {
            return targetAngle < shootSpeedMultiplier * 0.2f;
        }
        else
        {
            return targetAngle > - shootSpeedMultiplier * 0.2f;
        }
        
    }

    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    private static Vector2 RotateAround(Vector2 center, Vector2 v, float degrees, float magnitude)
    {
        return SetVectorMagnitude(Rotate(v - center, degrees), magnitude) + center;
    }

    private static Vector2 SetVectorMagnitude(Vector2 v, float magnitude)
    {
        float ratio = magnitude / v.magnitude;
        v *= ratio;
        return v;
    }

    private void SetRotationDirection()
    {
        // if going counterclockwise reaches track faster, set counterclockwise to true
        counterclockwise = GetCurrentAngle() > 0;
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
            if (targetBallT + tStepSize > 1)
            {
                if (targetBallCurveIndex < GetNumberOfCurves(route) - 1)
                {
                    return GetLocationRelativeToBall(targetBallPos, 0, radius, targetBallRadius, tStepSize, errorSize, route, targetBallCurveIndex + 1, aheadOfGivenBall);
                } else
                {
                    return new Location(1, targetBallCurveIndex);
                }
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


    private void ClearCheck()
    {
        List<GameObject> balls = new();

        // iterate behind
        GameObject behind = this.behind;

        if (behind != null)
        {
            do
            {
                if (behind.GetComponent<Ball>().ballType != this.ballType) break;
                
                balls.Add(behind);
                behind = behind.GetComponent<Ball>().behind;
            } 
            while (behind != null);
        }
        

        // iterate ahead
        GameObject ahead = this.ahead;

        if (ahead != null)
        {
            do
            {
                if (ahead.GetComponent<Ball>().ballType != this.ballType) break;

                balls.Add(ahead);
                ahead = ahead.GetComponent<Ball>().ahead;
            }
            while (ahead != null);
        }


        if (balls.Count >= 2)
        {
            if (ahead != null)
            {
                ahead.GetComponent<Ball>().behind = behind;
            }
            if (behind != null)
            {
                behind.GetComponent<Ball>().ahead = ahead;
            }

            foreach (GameObject ball in balls)
            {
                Destroy(ball);
            }
            Destroy(this.gameObject);
        }
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

public class Position
{
    public Transform route;
    public float t = 0;
    public float speedMultiplier = 1f;
    public float shootSpeedMultiplier = 1f;
    public int currentCurveIndex = 0;
    public int totalCurves;

    public float stepSize = 0.005f;
    public float errorSize;
    public float tStepSize = 0.001f;

    public Position(Transform route)
    {
        if (route != null)
        {
            this.totalCurves = (route.childCount - 1) / 3;
        }
        errorSize = stepSize / 50;
    }

    public Position(Transform route, float t, int currentCurveIndex) : this(route)
    {
        this.t = t;
        this.currentCurveIndex = currentCurveIndex;
    }
}
