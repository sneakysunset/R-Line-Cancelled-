using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;
    private Vector2 playerVelocity;

    private float acceleration, deceleration;

    public float moveSpeed;
    public float ax, dx;
    public float airAx, airDx;
    public float maxXVelocity = 20;
    public float maxYVelocity = 20;

    private void Start()
    {
        player = GetComponent<Player>();
        rb = player.rb;
    }

    private void FixedUpdate()
    {
        Acceleration();
        ChangeDirection();
        if (player.canMove) Move();
        LimitVelocity();
    }

    private void Move()
    {
        Vector2 movementVector = rb.velocity;
        movementVector = transform.right * player.moveValue.x * moveSpeed * Time.deltaTime ;
        
        var acc = movementVector.x != 0 ? acceleration : deceleration;


        playerVelocity = Vector2.Lerp(rb.velocity, movementVector, acc * Time.deltaTime);
        playerVelocity = (Vector2)transform.right * playerVelocity.x + (Vector2)transform.up * transform.InverseTransformDirection(rb.velocity).y;
        Debug.DrawRay(transform.position, playerVelocity, Color.black, Time.deltaTime);
        print(playerVelocity);
        rb.velocity = playerVelocity;

        if (player.moveValue != Vector2.zero && player.groundCheck) player.moving = true;
        else player.moving = false;
    }

    private void Acceleration()
    {
        if (player.groundCheck)
        {
            acceleration = ax;
            deceleration = dx;
        }
        else if(player.wallJumpCheck)
        {
            acceleration = airAx;
            deceleration = airDx;
        }
    }

    private void ChangeDirection()
    {
        if (player.moveValue.x < 0) player.rend.transform.rotation = Quaternion.Euler(0, 180, 0);
        if (player.moveValue.x > 0) player.rend.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void LimitVelocity() => rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxXVelocity, maxXVelocity), Mathf.Clamp(rb.velocity.y, -1000, maxYVelocity));
}
