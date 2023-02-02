using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ColHand : MonoBehaviour
{
    Item_Ball ballItemHeld;
    private Player player;
    [Range(-1, 1)] public float lineColliderNormalY;
    private void Update()
    {
    }

    public void OnLineTriggerExit(Collider2D col)
    {
        if(col.CompareTag("LineCollider"))
            Physics2D.IgnoreCollision(player.coll, col, false);
    }

    public void OnLineCollisionEnter(Collision2D col)
    {
        if (col.collider.CompareTag("LineCollider") && col.contacts[0].normal.y > lineColliderNormalY)
        {
            Physics2D.IgnoreCollision(player.coll, col.collider, true);
        }
    }
}
