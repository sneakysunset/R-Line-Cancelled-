using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Ball_ToPlayer : Item_Ball
{
    CharacterController2D[] charCs;
    public Transform target;
    public bool flying;
    float ogGravity;
    public float speed = 1;

    public override void Start()
    {
        base.Start();
        charCs = new CharacterController2D[2];
        charCs = FindObjectsOfType<CharacterController2D>();
        ogGravity = rb.gravityScale;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (flying)
        {
            rb.velocity = (target.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    public override void GrabStarted(Transform holdPoint)
    {
        base.GrabStarted(holdPoint);
        flying = false;
        target = null;
        rb.gravityScale = ogGravity;
    }

    public override void ThrowStarted(float throwStrength, CharacterController2D charC, ItemSystem iS)
    {
        setTagsLayers("Ball", "Ball", 7);

        iS.throwing = false;
        charC.canMove = true;
        charC.canJump = true;
        
        flying = true;

        tP.pointFolder.gameObject.SetActive(false);
        tP._line.positionCount = 1;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");

        rb.isKinematic = false;
        rb.gravityScale = 0;

        isHeld = false;
        iS.heldItem = null;

        if (charC != charCs[0]) target = charC.transform;
        else target = charCs[1].transform;
    }

    public override void ThrowHeld(float throwStrength, CharacterController2D charC){}

    public override void CancelThrow(){}

    public override void ThrowRelease(float throwStrength, CharacterController2D charC){}

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if ((flying && collision.collider.tag != "Ball" && collision.transform.tag != "Player") || collision.transform == target )
        {
            flying = false;
            target = null;
            rb.gravityScale = ogGravity;
        }
    }
}
