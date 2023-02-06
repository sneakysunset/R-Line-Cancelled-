using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Ball : Item
{
    [HideInInspector] public LineCreator lC;
     public BallType.BallCollisionType ballType;
    protected bool stuckToWall;
     public bool collideWithPlayer = true;
     public Player playerr;
    public override void Awake()
    {
        
        base.Awake();
        throwPreview = true;

    }

    public override void GrabStarted(Transform holdPoint, Player player)
    {
        playerr = player;
        if (!catchable)
        {
            GrabRelease(player);
            return;
        }
        base.GrabStarted(holdPoint, player);

        Physics2D.IgnoreCollision(player.coll, lC.lineC, true);

    }

    public override void GrabRelease(Player player)
    {
        base.GrabRelease(player);
        setTagsLayers("Ball", "Ball", 7);

        Physics2D.IgnoreCollision(player.coll, lC.lineC, false);
    }

    public override void ThrowStarted(float throwStrength, Player player)
    {
        base.ThrowStarted(throwStrength, player);
        stuckToWall = false;
    }

    public override void ThrowRelease(float throwStrength, Player player)
    {
        base.ThrowRelease(throwStrength, player);
        setTagsLayers("Ball", "Ball", 7);

        Physics2D.IgnoreCollision(player.coll, lC.lineC, false);

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        lC.LineUpdater();

        if (stuckToWall && ballType == BallType.BallCollisionType.stickToWall)
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
        if (ballType == BallType.BallCollisionType.hardBounce)
        {
            rb.AddForce(new Vector2(0, 2), ForceMode2D.Impulse);
        }
    }

    public virtual void OnDestroy()
    {
        if(isHeld) Physics2D.IgnoreCollision(playerr.coll, lC.lineC, true);
    }
}
