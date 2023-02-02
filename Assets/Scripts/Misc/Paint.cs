using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    public BallType.BallThrowType paintThrowType;
    public BallType.BallCollisionType paintCollisionType;
    public bool catchable;
    private int ID;
    private void Awake()
    {
        ID = Random.Range(0, 999999);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.parent && collision.transform.parent.TryGetComponent(out BallType ballType) && ballType.ID != ID)
        {
            ballType.changeBallType(paintThrowType, paintCollisionType, catchable);
            ballType.ID = ID;
        }
    }
}
