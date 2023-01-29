using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Player : Item
{
    PlayerCollisionManager pCM;
    CharacterController2D myCharC;

    public override void Start()
    {
        base.Start();
        col = transform.Find("Collider").GetComponent<Collider2D>();
        pCM = GetComponent<PlayerCollisionManager>();
        myCharC = GetComponent<CharacterController2D>();
    }

    public override void GrabStarted(Transform holdPoint)
    {
        base.GrabStarted(holdPoint);
        myCharC.canMove = false;
        myCharC.canJump = false;
    }

    public override void GrabRelease()
    {
        base.GrabRelease();
        tag = "Player";
        col.tag = "Player";
        col.gameObject.layer = 9;
        myCharC.canMove = true;
        myCharC.canJump = true;
    }

    public override void ThrowHeld(float throwStrength, CharacterController2D charC)
    {
        if(charC.moveValue != Vector2.zero)
            tP.Sim(throwStrength * new Vector2(charC.moveValue.x, -1).normalized);
        else tP.Sim(throwStrength * charC.moveValue);
        myCharC.canJump = false;
        myCharC.canMove = false;
    }

    public override void ThrowRelease(float throwStrength, CharacterController2D charC)
    {
        charC.canMove = true;
        tP.pointFolder.gameObject.SetActive(false);
        rb.isKinematic = false;
        tP._line.positionCount = 1;
        rb.velocity = new Vector2(charC.moveValue.x, -1).normalized * throwStrength;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");
        isHeld = false;

        tag = "Player";
        col.tag = "Player";
        col.gameObject.layer = 9;
        charC.canMove = true;
        charC.canJump = true;
    }

    public override void CancelThrow()
    {
        base.CancelThrow();
        myCharC.canJump = true;
        myCharC.canMove = true;
    }
}
