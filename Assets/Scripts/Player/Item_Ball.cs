using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Ball : Item
{
    protected LineCreator lC;
    public bool stickToWall;
    protected bool stuckToWall;
    public override void Start()
    {
        base.Start();
        lC = GetComponent<LineCreator>();
    }

    public override void GrabRelease()
    {
        base.GrabRelease();
        setTagsLayers("Ball", "Ball", 7);
    }

    public override void ThrowStarted(float throwStrength, CharacterController2D charC, ItemSystem iS)
    {
        base.ThrowStarted(throwStrength, charC, iS);
        stuckToWall = false;
    }

    public override void ThrowRelease(float throwStrength, CharacterController2D charC)
    {
        base.ThrowRelease(throwStrength, charC);
        setTagsLayers("Ball", "Ball", 7);

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        lC.LineUpdater();

        if (stuckToWall && stickToWall)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Le paramètre global "VolumeColBall" sert à changer le volume de l'event de collision de balle. Sa valeur est déterminé par la vitesse de la balle au moment de l'impact.
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("VolumeColBall", rb.velocity.magnitude);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Ball/Collision");
        if (collision.collider.tag != "Ball" && collision.transform.tag != "Player") stuckToWall = true;

    }
}
