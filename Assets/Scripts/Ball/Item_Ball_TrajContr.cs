using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Ball_TrajContr : Item_Ball
{
    Player[] players;
    Player playera, otherPlayera;
    public bool flying;
    float ogGravity;
    public float speed = 1;
    public float deviationSpeed = 1;
    Player playerC;
    Vector2 direction;
    public override void Start()
    {
        base.Start();
        players = new Player[2];
        players = FindObjectsOfType<Player>();
        ogGravity = rb.gravityScale;
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
        direction = (transform.position - player.transform.position).normalized;

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

    public override void ThrowHeld(float throwStrength, Player player) => rb.velocity = (new Vector2(direction.x * speed, player.moveValue.y * deviationSpeed)) * Time.deltaTime;

    public override void CancelThrow() => stopFlying();

    public override void ThrowRelease(float throwStrength, Player player) => stopFlying();

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        bool c1 = flying && collision.collider.tag != "Ball" && collision.transform.tag != "Player";
        bool c2 = collision.transform == otherPlayera?.transform;
        if (c1 || c2) stopFlying();
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


}
