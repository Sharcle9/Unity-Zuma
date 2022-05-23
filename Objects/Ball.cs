using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject prefab;
    public bool facingPlayer = false;
    private Vector2 pos;
    public BallType ballType = 0;

    public Ball()
    {
        pos = new Vector2(0f, 0f);
    }

    public Ball(BallType ballType) : this()
    {
        this.ballType = ballType;
        
    }

    public Ball(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public GameObject ToGameObject()
    {
        GameObject gameObject = Instantiate(prefab, pos, Quaternion.identity);

        return gameObject;
    }
}
