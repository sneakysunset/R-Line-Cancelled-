using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WallJump : MonoBehaviour
{
    private Player player;
    private Player_Jump playerJ;
    private Rigidbody2D rb;
    [Range(0f, 1f)] public float xWallJump = .7f;

    private void Start()
    {
        player = GetComponent<Player>();
        playerJ = GetComponent<Player_Jump>();
        rb = player.rb;
    }

    public void WJColEnter(Collision2D col)
    {
        if (Mathf.Abs(col.contacts[0].normal.x) > xWallJump && col.gameObject.CompareTag("Jumpable"))
        {
            player.jumpingInput = false;
            if (col.contacts[0].normal.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                playerJ.wallJumpDirection.x = -Mathf.Abs(playerJ.wallJumpDirection.x);
            } 
            if (col.contacts[0].normal.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                playerJ.wallJumpDirection.x = Mathf.Abs(playerJ.wallJumpDirection.x);
            }
            player.wallJumpCheck = true;
        }
    }

    public void WJColStay(Collision2D col)
    {
        if (Mathf.Abs(col.contacts[0].normal.x) > xWallJump && col.gameObject.CompareTag("Jumpable"))
        {
            rb.velocity = new Vector2(0, 0);
        }
    }
}
