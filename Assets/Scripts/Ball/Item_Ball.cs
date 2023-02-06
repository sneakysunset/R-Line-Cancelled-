using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Ball : Item
{
    #region variables
    [HideInInspector] public LineCreator lC;
    public BallType.BallCollisionType ballColType;
    public BallType.BallThrowType ballThrowType;
    protected bool stuckToWall;
    private float direction;
    private bool flying;
    public float flyingTrajSpeed = 500;
    public float flyingTrajDeviationSpeed = 700;
    public float flyingToPlayerSpeed = 500;
    private Player[] players;
    private float ogGravity;
    [HideInInspector] public bool collideWithPlayer = true;
    [HideInInspector] public Player _player;
    private Player _otherPlayer;
    #endregion

    #region UnityBaseEvents
    public override void Awake()
    {
        
        base.Awake();
        ogGravity = rb.gravityScale;
        throwPreview = true;
        players = new Player[2];
        players = FindObjectsOfType<Player>();

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        lC.LineUpdater();

        if (stuckToWall && ballColType == BallType.BallCollisionType.stickToWall)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }

        if (flying)
        {
            if(ballThrowType == BallType.BallThrowType.targetPlayer)
            {
                rb.velocity = (_otherPlayer.transform.position - transform.position).normalized * flyingToPlayerSpeed * Time.deltaTime;
            }
            else if (ballThrowType == BallType.BallThrowType.controlTarget)
            {
                rb.velocity = new Vector2(direction * flyingTrajSpeed, _player.moveValue.normalized.y * flyingTrajDeviationSpeed * Time.deltaTime);
            }
        }
    }
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Le paramètre global "VolumeColBall" sert à changer le volume de l'event de collision de balle. Sa valeur est déterminé par la vitesse de la balle au moment de l'impact.
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("VolumeColBall", rb.velocity.magnitude);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Ball/Collision");

        if (collision.collider.tag != "Ball" && collision.transform.tag != "Player") stuckToWall = true;
        if (ballColType == BallType.BallCollisionType.hardBounce)
        {
            rb.AddForce(new Vector2(0, 2), ForceMode2D.Impulse);
        }

        if (ballThrowType == BallType.BallThrowType.targetPlayer || ballThrowType == BallType.BallThrowType.controlTarget)
            StopFlying();
        else if (ballThrowType == BallType.BallThrowType.straightLine) rb.gravityScale = ogGravity;

        if (ballThrowType == BallType.BallThrowType.straightLine) flying = false;
    }

    public virtual void OnDestroy()
    {
        if(isHeld) Physics2D.IgnoreCollision(_player.coll, lC.lineC, true);
    }

    #endregion


    #region Grab Events
    public override void GrabStarted(Transform holdPoint, Player player)
    {
        _player = player;
        if (!catchable)
        {
            GrabRelease(player);
            return;
        }
        base.GrabStarted(holdPoint, player);
        flying = false;
        rb.gravityScale = ogGravity;
        Physics2D.IgnoreCollision(player.coll, lC.lineC, true);
    }

    public override void GrabRelease(Player player)
    {
        base.GrabRelease(player);
        setTagsLayers("Ball", "Ball", 7);

        Physics2D.IgnoreCollision(player.coll, lC.lineC, false);
        
    }
    #endregion

    #region ThrowEvents
    public override void ThrowStarted(float throwStrength, Player player)
    {
        switch (ballThrowType)
        {
            case BallType.BallThrowType.normal:
                base.ThrowStarted(throwStrength, player);
                break;
            case BallType.BallThrowType.targetPlayer:
                ThrowStarted_TargetPlayer(player);
                break;
            case BallType.BallThrowType.controlTarget:
                ThrowStarted_TargetContr(player);
                break;
            case BallType.BallThrowType.straightLine:
                base.ThrowStarted(throwStrength, player);
                rb.gravityScale = 0;
                break;
            default:
                base.ThrowStarted(throwStrength, player);
                break;
        }
            
            stuckToWall = false;
    }

    public override void ThrowHeld(float throwStrength, Player player)
    {
        switch (ballThrowType)
        {
            case BallType.BallThrowType.normal:
                base.ThrowHeld(throwStrength, player);
                break;
            case BallType.BallThrowType.targetPlayer:
                
                break;
            case BallType.BallThrowType.controlTarget:
                
                break;
            case BallType.BallThrowType.straightLine:
                tP.Sim(throwStrength * player.moveValue, false);
                break;
        }
    }

    public override void CancelThrow()
    {
        if (ballThrowType == BallType.BallThrowType.targetPlayer || ballThrowType == BallType.BallThrowType.controlTarget)
            StopFlying();
        else base.CancelThrow();
    }

    public override void ThrowRelease(float throwStrength, Player player)
    {
        if (ballThrowType == BallType.BallThrowType.targetPlayer || ballThrowType == BallType.BallThrowType.controlTarget)
            StopFlying();
        else
        {
            base.ThrowRelease(throwStrength, player);
            setTagsLayers("Ball", "Ball", 7);
            Physics2D.IgnoreCollision(player.coll, lC.lineC, false);
        }

        if (ballThrowType == BallType.BallThrowType.straightLine) flying = true;
    }

    private void ThrowStarted_TargetPlayer(Player player)
    {
        _player = player;
        player.holdableItems.Add(this);
        stuckToWall = false;
        flying = true;
        player.canMove = false;
        player.canJump = false;
        rb.isKinematic = false;
        rb.gravityScale = 0;

        if (player == players[0]) _otherPlayer = players[1];
        else _otherPlayer = players[0];
    }

    private void ThrowStarted_TargetContr(Player player)
    {
        _player = player;
        player.holdableItems.Add(this);
        stuckToWall = false;
        flying = true;
        player.canMove = false;
        player.canJump = false;
        rb.isKinematic = false;
        rb.gravityScale = 0;

        if ((transform.position - player.transform.position).normalized.x < 0) direction = -1;
        else direction = 1;
    }
    #endregion

    #region OtherEvents
    private void StopFlying()
    {
        _player.throwing = false;
        _player.canMove = true;
        _player.canJump = true;
        Physics2D.IgnoreCollision(_player.coll, col, false);
        isHeld = false;

        _player.heldItem = null;
        flying = false;
        _player = null;
        _otherPlayer = null;
        rb.gravityScale = ogGravity;
        setTagsLayers("Ball", "Ball", 7);
    }

    public void ChangeType()
    {
        if (!isHeld)
        {

        }

        if (flying)
        {
            switch (ballThrowType)
            {
                case BallType.BallThrowType.straightLine:
                    rb.gravityScale = 0;
                    break;

            }
        }
    }
    #endregion
}
