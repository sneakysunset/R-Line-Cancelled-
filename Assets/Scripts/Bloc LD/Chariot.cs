using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chariot : MonoBehaviour
{
    CharacterController2D charC;
    Rigidbody2D rb;
    public float pushStrength;
    public float rCRange;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && charC?.transform != collision.transform)
        {
            charC = collision.transform.GetComponent<CharacterController2D>();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && charC.moveValue.x > .3f)
        {
           rb.velocity += new Vector2(charC.moveValue.x * pushStrength * Time.deltaTime, 0);
        } 
    }
}
