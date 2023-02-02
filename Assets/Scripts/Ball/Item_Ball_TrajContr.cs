using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Ball_TrajContr : Item_Ball
{
    CharacterController2D[] charCs;
    CharacterController2D chara, oChara;
    public bool flying;
    float ogGravity;
    public float speed = 1;
    public float deviationSpeed = 1;
    ItemSystem itemSystem;
    Vector2 direction;
    public override void Start()
    {
        base.Start();
        charCs = new CharacterController2D[2];
        charCs = FindObjectsOfType<CharacterController2D>();
        ogGravity = rb.gravityScale;
    }

    public override void FixedUpdate()
    {
        if(!flying) base.FixedUpdate();
        else lC.LineUpdater();
    }

    public override void ThrowStarted(float throwStrength, CharacterController2D charC, ItemSystem iS)
    {
        itemSystem = iS;
        itemSystem.closestItem.holdableItems.Add(this);
        direction = (transform.position - charC.transform.position).normalized;

        stuckToWall = false;
        flying = true;
        charC.canMove = false;
        charC.canJump = false;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");

        rb.isKinematic = false;
        rb.gravityScale = 0;

        if (charC == charCs[0])
        {
            chara = charCs[0];
            oChara = charCs[1];
        }
        else
        {
            chara = charCs[1];
            oChara = charCs[0];
        }
    }

    public override void ThrowHeld(float throwStrength, CharacterController2D charC) => rb.velocity = (new Vector2(direction.x * speed, chara.moveValue.y * deviationSpeed)) * Time.deltaTime;

    public override void CancelThrow() => stopFlying();

    public override void ThrowRelease(float throwStrength, CharacterController2D charC) => stopFlying();

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        bool c1 = flying && collision.collider.tag != "Ball" && collision.transform.tag != "Player";
        bool c2 = collision.transform == oChara?.transform;
        if (c1 || c2) stopFlying();
    }

    void stopFlying()
    {
        itemSystem.throwing = false;
        chara.canMove = true;
        chara.canJump = true;
        isHeld = false;

        itemSystem.heldItem = null;
        flying = false;
        chara = null;
        oChara = null;
        rb.gravityScale = ogGravity;
        itemSystem = null;
        setTagsLayers("Ball", "Ball", 7);
    }


}
