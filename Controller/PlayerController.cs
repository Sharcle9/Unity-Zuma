using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* sprite rotation correction, if any */
    public float additionalRotationRad = 0.785f;

    public Vector2 playerPos = new Vector2(0f, 0f);

    private PrefabController prefabController;
    private GameObject[] BallPrefabs;
    public BallType ballType;
    private Vector2 mousePos;
    private Transform ballQueue;
    private GameObject currentBallObject;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = playerPos;
        prefabController = new PrefabController();
        BallPrefabs = prefabController.BallPrefabs;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        FollowCursor();

        if (Input.GetButtonDown("Fire1"))
        {
            currentBallObject = LaunchBall();
            ballType = (BallType)Random.Range(0, 5);
            this.GetComponent<SpriteRenderer>().sprite = BallPrefabs[(int)ballType].GetComponent<SpriteRenderer>().sprite;
        }
        
    }

    private void FollowCursor()
    {
        float x = mousePos.x;
        float y = mousePos.y;

        float rotationRad = 0;

        if (x == 0)
        {
            rotationRad = y >= 0 ? 0 : Mathf.PI;
        }
        else if (y == 0)
        {
            rotationRad = x >= 0 ? Mathf.PI / 2 : Mathf.PI * 3 / 2;
        }
        else
        {
            rotationRad = (float) System.Math.Atan(x / y) + (y > 0 ? 0 : Mathf.PI);
        }

        transform.rotation = Quaternion.Euler(0, 0, - (rotationRad + additionalRotationRad) * Mathf.Rad2Deg);

        if (Input.GetButtonDown("Fire1"))
        {
            // Debug.Log(rotationRad);
        }
        
    }

    private GameObject LaunchBall()
    {
        GameObject ball = Instantiate(BallPrefabs[(int) ballType], this.transform.parent);
        ball.transform.rotation = this.transform.rotation;
        ball.AddComponent<Ball>();
        ball.GetComponent<Ball>().Init(this.transform.position, mousePos, ballQueue, 0.42f, ballType);

        return ball;
    }


    public void SetBallQueue(Transform ballQueue)
    {
        this.ballQueue = ballQueue;
    }
}
