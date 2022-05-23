using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapData : ScriptableObject
{
    public List<GameObject> ControlPoints = new();

    public MapData(List<GameObject> ControlPoints)
    {
        foreach (var item in ControlPoints)
        {
            this.ControlPoints.Add(item);
        }
    }

    public void SetControlPoints(List<GameObject> ControlPoints)
    {
        foreach (var item in ControlPoints)
        {
            this.ControlPoints.Add(item);
        }
    }

    public Vector3 GetPosition(float position)
    {
        int index = Mathf.FloorToInt(position);

        return Vector3.Lerp(ControlPoints[index].transform.position, ControlPoints[index + 1].transform.position, position - index);
    }

}
