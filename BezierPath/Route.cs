using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Route : MonoBehaviour
{

    public List<GameObject> ControlPoints;
    private List<Vector2> TargetPoints;
    public int segmentSize = 20;
    public bool showDrawing = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrawGizmos()
    {
        ControlPoints.Clear();

        foreach (Transform item in transform)
        {
            ControlPoints.Add(item.gameObject);
        }

        List<Vector3> GizmosPoints = GetGizmosPointsPos(ControlPoints, segmentSize);

        foreach (Vector3 point in GizmosPoints)
        {
            Gizmos.DrawSphere(point, 0.25f);
        }

    }

    public List<Vector3> GetPosFromObjects(List<GameObject> objects)
    {
        return objects.Select(i => i.transform.position).ToList();
    }

    public List<Vector3> GetGizmosPointsPos(List<GameObject> ControlPoints, int segmentSize)
    {
        List<Vector3> ControlPointsPos = GetPosFromObjects(ControlPoints);
        List<Vector3> GizmosPoints = new();

        for (int i = 0; i < ControlPointsPos.Count - 3; i += 3)
        {
            Vector3 p0 = ControlPointsPos[i];
            Vector3 p1 = ControlPointsPos[i + 1];
            Vector3 p2 = ControlPointsPos[i + 2];
            Vector3 p3 = ControlPointsPos[i + 3];

            for (int j = 0; j < segmentSize; j++)
            {
                GizmosPoints.Add(BezierCubicFormula((float)j / segmentSize, p0, p1, p2, p3));
            }
        }

        return GizmosPoints;
    }

    public Vector3 BezierCubicFormula(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return p0 * Mathf.Pow((1 - t), 3) + (3 * Mathf.Pow((1 - t), 2) * t * p1) + ((1 - t) * 3 * Mathf.Pow(t, 2) * p2) + p3 * Mathf.Pow(t, 3);
    }
}
