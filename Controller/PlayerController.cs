using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* sprite rotation correction, if any */
    private float additionalRotationRad = 0.785f;
    private float offset = 0.03f;

    public Vector2 ballPos = new(0f, 0f);
    public Vector2 playerPosFacingRight;
    public Vector2 playerPosFacingLeft;

    private bool isFacingRight = true;
    private PrefabController prefabController;
    private GameObject[] BallPrefabs;
    public BallType ballType;
    private Vector2 mousePos;
    private Transform ballQueue;
    private GameObject playerObject;
    private GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = ballPos;
        prefabController = new PrefabController();
        BallPrefabs = prefabController.BallPrefabs;
        playerObject = Instantiate(prefabController.player, this.transform);
        ballType = (BallType)Random.Range(0, 5);
        playerPosFacingRight = ballPos + new Vector2(-0.2f, 0.3f);
        playerPosFacingLeft = ballPos + new Vector2(0.2f, 0.3f);
        ball = Instantiate(prefabController.BallPrefabs[(int)ballType], this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        FollowCursor();

        if (Input.GetButtonDown("Fire1"))
        {
            LaunchBall();
            ballType = (BallType)Random.Range(0, 5);
            this.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BallPrefabs[(int)ballType].GetComponent<SpriteRenderer>().sprite;
        }
        
    }

    private void FollowCursor()
    {
        float x = mousePos.x;
        float y = mousePos.y;

        if (isFacingRight)
        {
            if (x <= 0)
            {
                playerObject.transform.position = new Vector2(0.2f, 0.3f);
                playerObject.transform.localScale = new Vector2(-1f, 1f);
                isFacingRight = false;
            }
            Vector2 u = mousePos - playerPosFacingRight;
            playerObject.transform.position = new Vector2(- u.x * offset / 2, - u.y * offset / 5) + playerPosFacingRight;
        } 
        else
        {
            if (x > 0)
            {
                playerObject.transform.position = new Vector2(-0.2f, 0.3f);
                playerObject.transform.localScale = new Vector2(1f, 1f);
                isFacingRight = true;
            }

            Vector2 u = mousePos - playerPosFacingLeft;
            playerObject.transform.position = new Vector2(- u.x * offset / 2, - u.y * offset / 5) + playerPosFacingLeft;
        }

        float rotationRad;

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

        ball.transform.rotation = Quaternion.Euler(0, 0, - (rotationRad + additionalRotationRad) * Mathf.Rad2Deg);

        Vector2 v = mousePos - ballPos;
        ball.transform.position = v * offset + ballPos;



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
