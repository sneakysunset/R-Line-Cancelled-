using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chariot : MonoBehaviour
{
    Player player;
    Rigidbody2D rb;
    public float pushStrength;
    public float rCRange;
    public Transform cargo;
    public Transform cargoPoint;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cargo.position = cargoPoint.position;
        cargo.parent = transform;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && player?.transform != collision.transform)
        {
            player = collision.transform.GetComponent<Player>();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && player.moveValue.x > .3f)
        {
           rb.velocity += new Vector2(player.moveValue.x * pushStrength * Time.deltaTime, 0);
        } 
    }
}
