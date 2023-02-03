using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallType : MonoBehaviour
{
    public enum BallThrowType { normal, targetPlayer, controlTarget, nBounceBeforeDestroy, bolt}
    public enum BallCollisionType { normal, stickToWall, hardBounce}
    public BallThrowType ballThrowType;
    public BallCollisionType ballCollisionType;
    public bool catchable;
    Item_Ball item;
    public GameObject Line;
    public bool collideWithPlayer;
    [HideInInspector] public int ID;
    private void Awake()
    {
        ID = Random.Range(0, 999999);
        changeBallType(ballThrowType, ballCollisionType, catchable);
        GetComponent<LineCreator>().linePrefab = Line;
    }

    public void changeBallType(BallThrowType bTT, BallCollisionType bCT, bool cat)
    {
        bool isHeld = false;
        Transform holdPoint = null;
        Player player = null;
        GameObject highlight = null;
        Rigidbody2D rb = null;
        ThrowPreview tP = null;
        Collider2D col = null;
        LineCreator lC = null;
        if(TryGetComponent(out Item_Ball currentItem))
        {
            isHeld = currentItem.isHeld;
            if(isHeld)
            {
                player = currentItem.playerr;
                holdPoint = currentItem.heldPoint;
                highlight = currentItem.Highlight;
                rb = currentItem.rb;
                tP = currentItem.tP;
                col = currentItem.col;
                lC = currentItem.lC;
            }
            Destroy(currentItem);
        }

        switch (bTT)
        {
            case BallThrowType.normal:
                item = gameObject.AddComponent<Item_Ball>();
                break;
            case BallThrowType.targetPlayer:
                item = gameObject.AddComponent<Item_Ball_ToPlayer>();
                break;
            case BallThrowType.controlTarget:
                item = gameObject.AddComponent<Item_Ball_TrajContr>();
                break;
            case BallThrowType.nBounceBeforeDestroy:
                item = gameObject.AddComponent<Item_Ball>();
                break;
            case BallThrowType.bolt:
                item = gameObject.AddComponent<Item_Ball>();
                break;
            default:
                item = gameObject.AddComponent<Item_Ball>();
                break;
        }

        if (isHeld)
        {


            item.isHeld = true;
            item.heldPoint = holdPoint;
            player.heldItem = item;
            item.playerr = player;


            item.rb = rb;
            item.Highlight = highlight;
            item.tP = tP;
            item.lC = lC;
            item.col = col;
            item.generateLine = true;
            item.setTagsLayers("Held", "Held", 14);
            Physics2D.IgnoreCollision(player.coll, item.lC.lineC, true);
            item.rb.isKinematic = true;
            item.Highlight.SetActive(true);
            item.collideWithPlayer = collideWithPlayer;
        }
        else
        {
            item.generateLine = true;
            item.Highlight = transform.Find("Highlight").gameObject;
            item.rb = GetComponent<Rigidbody2D>();
            item.throwPreview = GetComponent<ThrowPreview>();
            item.col = GetComponentInChildren<Collider2D>(); ;
            item.lC = GetComponent<LineCreator>();
        }

        switch (bCT)
        {
            case BallCollisionType.normal:
                item.ballType = BallCollisionType.normal;
                break;
            case BallCollisionType.stickToWall:
                item.ballType = BallCollisionType.stickToWall;
                break;
            case BallCollisionType.hardBounce:
                item.ballType = BallCollisionType.hardBounce;
                break;
            default:
                item.ballType = BallCollisionType.normal;
                break;
        }

        if (cat) item.catchable = true;
        else item.catchable = false;

    }

}
