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
    private int ballType = 3;
    private Vector2 mousePos;


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
            LaunchBall();
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

    private void LaunchBall()
    {
        GameObject ball = Instantiate(BallPrefabs[ballType], this.transform.parent.transform);
        ball.transform.rotation = this.transform.rotation;
        ball.AddComponent<Ball>();
        ball.GetComponent<Ball>().Init(this.transform.position, mousePos, true);
    }

    public void ToGameObject()
    {
        GameObject gameObject = new GameObject("Unspecified Ball");
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

    }
}
