using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Player : Item
{
    Player myPlayer;

    public override void Start()
    {
        base.Start();
        myPlayer = GetComponent<Player>();
    }

    public override void GrabStarted(Transform holdPoint, Player player)
    {
        base.GrabStarted(holdPoint, player);
        myPlayer.canMove = false;
        myPlayer.canJump = false;
    }

    public override void GrabRelease(Player player)
    {
        base.GrabRelease(player);
        setTagsLayers("Player", "Player", 9);
        myPlayer.canMove = true;
        myPlayer.canJump = true;
    }

    public override void ThrowHeld(float throwStrength, Player player)
    {
        if(player.moveValue != Vector2.zero)
            tP.Sim(throwStrength * new Vector2(player.moveValue.x, -1).normalized);
        else tP.Sim(throwStrength * player.moveValue);
    }

    public override void ThrowRelease(float throwStrength, Player player)
    {
        player.canMove = true;
        tP.pointFolder.gameObject.SetActive(false);
        rb.isKinematic = false;
        tP._line.positionCount = 1;
        rb.velocity = new Vector2(player.moveValue.x, -1).normalized * throwStrength;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");
        isHeld = false;
        Physics2D.IgnoreCollision(player.coll, col, false);
        setTagsLayers("Player", "Player", 9);
        player.canMove = true;
        player.canJump = true;
    }

    public override void CancelThrow()
    {
        base.CancelThrow();
        myPlayer.canJump = true;
        myPlayer.canMove = true;
    }
}
