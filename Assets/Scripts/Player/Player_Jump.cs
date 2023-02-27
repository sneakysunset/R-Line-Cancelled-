using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Jump : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;
    public float fallMult, lowJumpMult;
    public float jumpStrength, secondJumpStrength;
    public Vector2 wallJumpDirection;
    public float wallJumpStrength;
    [Range(-1f, 1f)] public float yGroundCheck = .1f;
    //Prevent groundCheck = true on the frame the player jumps
    bool jumpCheck;
 
    private void Start()
    {
        player = GetComponent<Player>();
        rb = player.rb;
    }

    private void FixedUpdate()
    {
        jumpCheck = true;
        if (player.jumpingInput && (player.groundCheck || player.numOfJump > 0) && player.canJump && jumpCheck && player.jumpChecker) Jump();
        else if (!player.groundCheck) Falling();
        player.groundCheck = false;
    }

    void Jump()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Jump");
        player.jumpChecker = false;

        if (player.groundCheck) player.numOfJump = 1;
        else player.numOfJump = 0;

        //if(player.wallJumpCheck) rb.AddForce(wallJumpDirection.normalized * wallJumpStrength, ForceMode2D.Impulse);
        //else rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
    }

    void Falling()
    {
        if (rb.velocity.y < 0) rb.velocity += Vector2.up * Time.deltaTime * Physics2D.gravity.y * (fallMult - 1);
        else if (rb.velocity.y > 0 && !player.jumpingInput) rb.velocity += Vector2.up * Time.deltaTime * Physics2D.gravity.y * (lowJumpMult - 1);
    }

    public void GroundCheck(Collision2D col)
    {
        if (col.contacts[0].normal.y > yGroundCheck && jumpCheck)
        {
            player.groundCheck = true;
            player.numOfJump = 2;
            player.wallJumpCheck = false;
        }
    }
}
