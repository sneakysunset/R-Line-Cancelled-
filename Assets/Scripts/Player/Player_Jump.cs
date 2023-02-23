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
        else if (!player.groundCheck) Falling();
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

    void Falling()
    {
       
        //if ((rb.velocity * transform.up).magnitude < 0) rb.velocity += (Vector2)transform.up * Time.deltaTime * Physics2D.gravity.magnitude * (fallMult - 1);
        //else if ((rb.velocity * transform.up).magnitude > 0 && !player.jumpingInput) rb.velocity += (Vector2)transform.up * Time.deltaTime * Physics2D.gravity.magnitude * (lowJumpMult - 1);
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
        if (player.groundCheck && player.moving)
        {
            Physics2D.queriesHitTriggers = false;
            player.coll.isTrigger = true;
            RaycastHit2D hit = Physics2D.Raycast(rayCastPoint.position, (-transform.up + .5f * player.rend.transform.right).normalized, 1f);
            player.coll.isTrigger = false;
            Physics2D.queriesHitTriggers = true;
            if (hit)
            {
                gravityFlatValue = Physics2D.gravity.magnitude * (Vector2)hit.normal.normalized;
                Debug.DrawRay(hit.point, gravityFlatValue, Color.yellow, Time.deltaTime);
                transform.up = hit.normal.normalized;
            }
            else
            {
                hit = Physics2D.Raycast(transform.position, -transform.up, 1f);
                if (hit)
                {
                    gravityFlatValue = Physics2D.gravity.magnitude * (Vector2)hit.normal.normalized;
                    transform.up = hit.normal.normalized;
                }
            }
        }
        else if (player.groundCheck)
        {
            gravityFlatValue = Physics2D.gravity.magnitude * (Vector2)transform.up;
        }
        else if ((rb.velocity * transform.up).magnitude < 0)
        {
            gravityFlatValue = Vector2.up * Physics2D.gravity.y * (fallMult - 1); ;
            gravity = Vector2.up * Physics2D.gravity.y * (fallMult - 1); ;

            transform.up = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * 10);
        }
        else if ((rb.velocity * transform.up).magnitude > 0 && !player.jumpingInput)
        {
            gravityFlatValue = (Vector2)transform.up * -Physics2D.gravity.y * (lowJumpMult - 1);
            gravity = (Vector2)transform.up * -Physics2D.gravity.y * (lowJumpMult - 1);
        }

        gravity = Vector2.Lerp(gravity, gravityFlatValue, Time.deltaTime * 10f);
         
        rb.velocity -= gravity * gravityScale * Time.deltaTime;

        //Debug.DrawRay(rayCastPoint.position,( -transform.up + .5f * player.rend.transform.right).normalized, Color.red, Time.deltaTime);
        //Debug.DrawRay(transform.position,gravity.normalized, Color.white, Time.deltaTime);
        //Debug.DrawRay(transform.position,gravity * Time.deltaTime, Color.blue, Time.deltaTime);
    }
}
