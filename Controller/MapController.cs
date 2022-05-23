using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    static int ballType = 0;
    public GameObject[] BallPrefabs;
    public MapData mapData;

    public MapController(MapData mapData)
    {
        this.mapData = mapData;
    }

    public BallType GenerateBall()
    {
        return (BallType) ballType;
    }

    public void GenerateBallOnClick()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float x = mousePos.x;
        float y = mousePos.y;
        GameObject ballObject = Instantiate(BallPrefabs[1], this.transform);
        ballObject.transform.position = new Vector3(x, y, 10f);
        ballObject.transform.parent = this.transform;
        ballObject.name = "ball";
    }

    public void GenerateQueue(int count)
    {
        BallQueue ballQueue = gameObject.AddComponent<BallQueue>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GenerateBallOnClick();
        }
    }

}
