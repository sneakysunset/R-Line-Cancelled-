using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Jump : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;
    public float fallMult, lowJumpMult;
    public float jumpStrength;
    public Vector2 wallJumpDirection;
    public float wallJumpStrength;
    [Range(-1f, 1f)] public float yGroundCheck = .1f;
    public float gravityScale = 1;
    private Transform rayCastPoint;
    private Vector2 gravity = Vector2.zero;
    //Prevent groundCheck = true on the frame the player jumps
    bool jumpCheck;

    private void Start()
    {
        player = GetComponent<Player>();
        rb = player.rb;
        rayCastPoint = player.rend.transform.Find("RayCastPoint");
    }

    private void FixedUpdate()
    {
        jumpCheck = true;
        if (player.jumpingInput && (player.groundCheck || player.wallJumpCheck) && player.canJump && jumpCheck) Jump();
        ArtificialGravity();
        player.groundCheck = false;
    }

    void Jump()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Jump");
        jumpCheck = false;
        //if(player.wallJumpCheck) rb.AddForce(wallJumpDirection.normalized * wallJumpStrength, ForceMode2D.Impulse);
        rb.AddForce(transform.up * jumpStrength, ForceMode2D.Impulse);
    }

    public void GroundCheck(Collision2D col)
    {
        if (/*col.contacts[0].normal.y > yGroundCheck &&*/ jumpCheck)
        {
            player.groundCheck = true;
            player.wallJumpCheck = false;
        }
    }


    void ArtificialGravity()
    {
        Vector2 gravityFlatValue = Physics2D.gravity.magnitude * Vector2.up;
        Physics2D.queriesHitTriggers = false;
        player.coll.isTrigger = true;
        RaycastHit2D hitRight = Physics2D.Raycast(rayCastPoint.position, (-transform.up + .5f * player.rend.transform.right).normalized, 1f);
        RaycastHit2D hitUpRight = Physics2D.Raycast(rayCastPoint.position, (transform.up + .5f * player.rend.transform.right).normalized, 1f);
        player.coll.isTrigger = false;
        Physics2D.queriesHitTriggers = true;
        if (player.groundCheck && player.moving)
        {
            if (hitRight)
            {
                gravityFlatValue = Physics2D.gravity.magnitude * (Vector2)hitRight.normal.normalized;
                Debug.DrawRay(hitRight.point, gravityFlatValue, Color.yellow, Time.deltaTime);
                transform.up = hitRight.normal.normalized;
            }
            else
            {
                hitRight = Physics2D.Raycast(transform.position, -transform.up, 1f);
                if (hitRight)
                {
                    gravityFlatValue = Physics2D.gravity.magnitude * (Vector2)hitRight.normal.normalized;
                    transform.up = hitRight.normal.normalized;
                }
            }
        }
        else if (player.groundCheck)
        {
            gravityFlatValue = Physics2D.gravity.magnitude * (Vector2)transform.up;
        }
        else if(!player.groundCheck && hitUpRight)
        {
            gravityFlatValue = Physics2D.gravity.magnitude * (Vector2)hitUpRight.normal.normalized;
            Debug.DrawRay(hitUpRight.point, gravityFlatValue, Color.yellow, Time.deltaTime);
            transform.up = hitUpRight.normal.normalized;
        }
        else if (rbIsOverY())
        {
            gravityFlatValue = (Vector2)transform.up * -Physics2D.gravity.y * (lowJumpMult - 1);
            gravity = (Vector2)transform.up * -Physics2D.gravity.y * (lowJumpMult - 1);
        }
        else if (!rbIsOverY() && !player.jumpingInput)
        {
            gravityFlatValue = Vector2.up * -Physics2D.gravity.y * (fallMult - 1); ;
            gravity = Vector2.up * -Physics2D.gravity.y * (fallMult - 1);

            transform.up = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * 10);
        }

        gravity = Vector2.Lerp(gravity, gravityFlatValue, Time.deltaTime * 10f);
         
        rb.velocity -= gravity * gravityScale * Time.deltaTime;

        Debug.DrawRay(rayCastPoint.position,( -transform.up + .5f * player.rend.transform.right).normalized, Color.red, Time.deltaTime);
        Debug.DrawRay(rayCastPoint.position,( transform.up + .5f * player.rend.transform.right).normalized, Color.red, Time.deltaTime);
        Debug.DrawRay(transform.position,gravity.normalized, Color.white, Time.deltaTime);
        
    }

    private bool rbIsOverY() 
    {
        bool result = false;
        if (rb.velocity == Vector2.zero) return false;
        float rbAngle = Vector2.SignedAngle(Vector2.right, rb.velocity.normalized);
        float trAngle = Vector2.SignedAngle(Vector2.right, transform.up);
        Vector2 gRb = new Vector2(Mathf.Cos((rbAngle + trAngle - 90) * 2 * Mathf.PI / 360  ), Mathf.Sin((rbAngle + trAngle - 90) * 2 * Mathf.PI / 360));
        if (gRb.y > 0) result = true;
        else result = false;

        print("Rb = " + gRb + " Angle = " + (rbAngle + trAngle));

        return result;
    }
}
