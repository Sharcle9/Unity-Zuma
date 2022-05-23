using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMove : MonoBehaviour
{
    [SerializeField]
    private Transform route;
    private float t;
    public float speedMultiplier = 1f;
    private int currentCurveIndex = 0;
    private int curvesRemaining;

    private float stepSize = 0.005f;
    private float errorSize;
    private float tStepSize = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        t = 0;
        stepSize *= speedMultiplier;
        errorSize = stepSize / 20;
        curvesRemaining = (route.childCount - 1) / 3;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        
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

    private Vector2 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return p0 * Mathf.Pow((1 - t), 3) + (3 * Mathf.Pow((1 - t), 2) * t * p1) + ((1 - t) * 3 * Mathf.Pow(t, 2) * p2) + p3 * Mathf.Pow(t, 3);
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
