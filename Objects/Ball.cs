using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject prefab;
    public bool facingPlayer = false;
    public bool isHead;
    public BallType ballType = 0;
    public GameObject prev = null;
    public GameObject next = null;
    private Transform route;
    private float t;
    public float speedMultiplier;
    public float shootSpeedMultiplier = 1f;
    private int currentCurveIndex = 0;
    private int curvesRemaining;

    private float stepSize;
    private float errorSize;
    private float tStepSize;
    private float ballRadius;

    private Vector2 mousePos;
    private Vector2 playerPos;
    private bool isFromPlayer = false;

    private void Update()
    {
        if (isFromPlayer) Shoot();
        else if (isHead) Move();
        else Follow();
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
        errorSize = stepSize / 20;
        curvesRemaining = (route.childCount - 1) / 3;
        transform.position = GetBezierPoint(0, route, 0);
    }

    public void Init(Vector2 playerPos, Vector2 mousePos, bool isFromPlayer)
    {
        t = 0;
        this.playerPos = playerPos;
        this.mousePos = mousePos;
        this.isFromPlayer = isFromPlayer;
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
        if (prev == null) return;

        Vector2 prevBallPos = prev.transform.position;

        
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

    private Vector2 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return p0 * Mathf.Pow((1 - t), 3) + (3 * Mathf.Pow((1 - t), 2) * t * p1) + ((1 - t) * 3 * Mathf.Pow(t, 2) * p2) + p3 * Mathf.Pow(t, 3);
    }

    private float GetNextTFollow(float currentT, Vector3 prevBallPos, float radius, float tStepSize, float errorSize, Transform route, int currentCurveIndex)
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

    private Vector2 GetBezierPoint(float t, Transform route)
    {
        return GetBezierPoint(t, route.GetChild(0).position, route.GetChild(1).position, route.GetChild(2).position, route.GetChild(3).position);
    }

    private Vector2 GetBezierPoint(float t, Transform route, int currentCurveIndex)
    {
        // Debug.Log(currentCurveIndex);
        int vertex0 = currentCurveIndex * 3;
        int vertex1 = currentCurveIndex * 3 + 1;
        int vertex2 = currentCurveIndex * 3 + 2;
        int vertex3 = currentCurveIndex * 3 + 3;
        return GetBezierPoint(t, route.GetChild(vertex0).position, route.GetChild(vertex1).position, route.GetChild(vertex2).position, route.GetChild(vertex3).position);
    }

}
