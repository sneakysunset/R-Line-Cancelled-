using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ColHand : MonoBehaviour
{
    Item_Ball ballItemHeld;
    private Player player;
    [Range(-1, 1)] public float lineColliderNormalY;
    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (player.moveValue.y >= -1 && player.moveValue.y < -.85f)
        {
            player.coll.gameObject.layer = LayerMask.NameToLayer("PlayerOff");
        }
        else if (!player.myItem.isHeld)
        {
            player.coll.gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    public void OnLineTriggerStay(Collider2D col)
    {
        if(col.CompareTag("LineCollider")) Physics2D.IgnoreCollision(player.coll, col, true);

    }

    public void OnLineTriggerExit(Collider2D col)
    {
        if (col.CompareTag("LineCollider"))
        {
            
            if (player.heldItem && player.heldItem.TryGetComponent(out Item_Ball it) && it.lC.edgeC == col) { }
            else
            {
                Physics2D.IgnoreCollision(player.coll, col, false);
            } 
        }
    }

    public void OnLineCollisionEnter(Collision2D col)
    {
        bool condition1 = col.gameObject.tag == "LineCollider";
        bool condition2 = col.contacts[0].normal.y > lineColliderNormalY;

        if (condition1 && !condition2)
        {
            Physics2D.IgnoreCollision(player.coll, col.collider, true);
            player.rb.velocity = -col.relativeVelocity;
        }
        else if (condition1 && !condition2)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCorde/LineLand");
        }
        else if (!condition1)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Land");
        }
    }


}
