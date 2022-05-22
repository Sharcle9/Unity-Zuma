using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform tf;

    /* sprite rotation correction, if any */
    public float additionalRotationRad;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowCursor();

        if (Input.GetButtonDown("Fire1"))
        {
            LaunchBall();
        }
        
    }

    private void FollowCursor()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

        tf.rotation = Quaternion.Euler(0, 0, - (rotationRad + additionalRotationRad) * Mathf.Rad2Deg);

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log(rotationRad);
        }
        
    }

    private void LaunchBall()
    {

    }
}
