using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapConfig : ScriptableObject
{
    public List<Vector3> TargetBallPointList = new List<Vector3>();

    public Vector3 GetPosition(float position)
    {
        int index = Mathf.FloorToInt(position);

        return Vector3.Lerp(TargetBallPointList[index], TargetBallPointList[index + 1], position - index);
    }

}
