using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    static int ballType = 0;
    public GameObject[] BallPrefabs;

    public BallType GenerateBall()
    {
        return (BallType) ballType;
    }

    public void GenerateOneBall()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float x = mousePos.x;
        float y = mousePos.y;
        GameObject ballObject = Instantiate(BallPrefabs[1], this.transform);
        ballObject.transform.position = new Vector3(x, y, 10f);
        ballObject.transform.parent = this.transform;
        ballObject.name = "ball";
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GenerateOneBall();
        }
    }
}
