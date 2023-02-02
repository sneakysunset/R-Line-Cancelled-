using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Ball_TrajContr : Item_Ball
{
    Player[] players;
    Player playera, otherPlayera;
    public bool flying;
    float ogGravity;
    public float speed = 5;
    public float deviationSpeed = 4;
    Player playerC;
    float direction;
    public override void Awake()
    {
        base.Awake();
        players = new Player[2];
        players = FindObjectsOfType<Player>();
        ogGravity = rb.gravityScale;
        throwPreview = false;
    }

    public override void FixedUpdate()
    {
        if(!flying) base.FixedUpdate();
        else lC.LineUpdater();
    }

    public override void ThrowStarted(float throwStrength, Player player)
    {
        playerC = player;
        player.holdableItems.Add(this);
        if ((transform.position - player.transform.position).normalized.x < 0)
            direction = -1;
        else direction = 1;

        stuckToWall = false;
        flying = true;
        player.canMove = false;
        player.canJump = false;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");

        rb.isKinematic = false;
        rb.gravityScale = 0;

        if (player == players[0])
        {
            playera = players[0];
            otherPlayera = players[1];
        }
        else
        {
            playera = players[1];
            otherPlayera = players[0];
        }
    }

    public override void ThrowHeld(float throwStrength, Player player) => rb.velocity = (new Vector2(direction * speed, player.moveValue.y * deviationSpeed));

    public override void CancelThrow() => stopFlying();

    public override void ThrowRelease(float throwStrength, Player player) => stopFlying();

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (flying) stopFlying();
        
    }

    void stopFlying()
    {
        playerC.throwing = false;
        playera.canMove = true;
        playera.canJump = true;
        Physics2D.IgnoreCollision(playera.coll, col, false);
        isHeld = false;

        playerC.heldItem = null;
        flying = false;
        playera = null;
        otherPlayera = null;
        rb.gravityScale = ogGravity;
        playerC = null;
        setTagsLayers("Ball", "Ball", 7);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (flying)
        {
            stopFlying();
        }
    }


}
