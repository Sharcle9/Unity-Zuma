using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

public class BezierPathController : MonoBehaviour
{

    public GameObject targetBallPrefab;
    public int segmentPerCurve;
    private List<GameObject> ControllerPointList = new List<GameObject>();
    private List<Vector3> TargetBallPointList = new List<Vector3>();
    public float diameter = 0.9f;

    public bool isShowDrawing = false;

    private void Awake()
    {
        foreach (var item in TargetBallPointList)
        {
            GameObject targetBall = Instantiate(targetBallPrefab, GameObject.Find("Orange_0").transform);
            targetBall.transform.position = item;
        }
    }

    private void OnDrawGizmos()
    {
        ControllerPointList.Clear();

        foreach (Transform item in transform)
        {
            ControllerPointList.Add(item.gameObject);
        }

        List<Vector3> pointPos = ControllerPointList.Select(point => point.transform.position).ToList();
        var pointsOnCurve = GetDrawPoint(pointPos, segmentPerCurve);

        Vector3 startPoint = pointsOnCurve[0];
        TargetBallPointList.Clear();
        TargetBallPointList.Add(startPoint);

        for (int k = 0; k < pointsOnCurve.Count; k++)
        {
            if (Vector3.Distance(startPoint, pointsOnCurve[k]) >= diameter)
            {
                startPoint = pointsOnCurve[k];
                TargetBallPointList.Add(startPoint);
            }
        }

        foreach (var item in ControllerPointList)
        {
            item.GetComponent<MeshRenderer>().enabled = isShowDrawing;
        }

        if (!isShowDrawing) return;

        Gizmos.color = Color.red;

        foreach (var item in TargetBallPointList)
        {
            Gizmos.DrawSphere(item, diameter / 2);
        }

        Gizmos.color = Color.blue;

        for (int i = 0; i < pointPos.Count - 1; i++)
        {
            Gizmos.DrawLine(pointPos[i], pointPos[i + 1]);
        }

        Gizmos.color = Color.yellow;

        for (int j = 0; j < pointsOnCurve.Count - 1; j++)
        {
            Gizmos.DrawLine(pointsOnCurve[j], pointsOnCurve[j + 1]);
        }
    }

    public List<Vector3> GetDrawPoint(List<Vector3> controlPoint, int segmentPerCurve)
    {
        List<Vector3> pointOnCurve = new List<Vector3>();

        for(int i = 0; i < controlPoint.Count - 3; i += 3)
        {
            var p0 = controlPoint[i];
            var p1 = controlPoint[i + 1];
            var p2 = controlPoint[i + 2];
            var p3 = controlPoint[i + 3];

            for (int j = 0; j < segmentPerCurve; j++)
            {
                float t = j / (float)segmentPerCurve;

                pointOnCurve.Add(BezierCubicFormula(t, p0, p1, p2, p3));
            }
        }

        return pointOnCurve;
    }

    private Vector3 BezierCubicFormula(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return p0 * Mathf.Pow((1 - t), 3) + 3 * p1 * t * Mathf.Pow((1 - t), 2) + 3 * p2 * Mathf.Pow(t, 2) * (1 - t) + p3 * Mathf.Pow(t, 3);
    }

    [CustomEditor(typeof(BezierPathController))]
    public class BezierEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Map"))
            {
                (target as BezierPathController).CreateMapAssets();
            }
        }
    }

    public void CreateMapAssets()
    {
        string assetsSavePath = "Assets/Maps/map.asset";
        MapConfig mapConfig = new MapConfig();

        foreach (var item in TargetBallPointList)
        {
            mapConfig.TargetBallPointList.Add(item);
        }

        AssetDatabase.CreateAsset(mapConfig, assetsSavePath);
        AssetDatabase.SaveAssets();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
