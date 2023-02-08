using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallType : MonoBehaviour
{
    public enum BallThrowType { normal, targetPlayer, controlTarget, straightLine}
    public enum BallCollisionType { normal, stickToWall, hardBounce}
    Item_Ball item;
    [HideInInspector] public int ID;


    private void Awake()
    {
        ID = Random.Range(0, 999999);
    }

    public void changeBallType(BallThrowType bTT, BallCollisionType bCT, bool cat)
    {
        item.ballColType = bCT;
        item.ballThrowType = bTT;

        if (cat) item.catchable = true;
        else item.catchable = false;

        item.ChangeType();
    }


}
