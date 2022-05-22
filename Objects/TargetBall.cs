using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBall : MonoBehaviour
{
    public float position = 0f;
    private GameManager gameManager;
    // Start is called before the first frame update
    public TargetBall Init(GameManager gm)
    {
        gameManager = gm;
        gameObject.SetActive(true);
        return this;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = gameManager.mapConfig.GetPosition(position);
    }

    public bool IsNotStartBall()
    {
        return position >= 1f;
    }

    public TargetBall PrevBall { get; set; }

    public TargetBall NextBall { get; set; }
}
