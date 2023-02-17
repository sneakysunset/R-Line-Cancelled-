using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Collante : Ball_State
{
    Transform ballContainer;
    bool isActive;
    float realGravityScale;
    private void Awake()
    {
        ballContainer = GameObject.Find("BallContainer").transform;
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        //sprite.color = color;
        isActive = true;
        realGravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        //Comportement
        if (gameObject.CompareTag("Held"))
        {
            transform.SetParent(ballContainer);
            rb.gravityScale = realGravityScale;

        }
        //Transition
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive)
        {
            transform.SetParent(collision.transform);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
    }

    private void OnDisable()
    {
        rb.gravityScale = realGravityScale;
        realGravityScale = 0f;

        transform.SetParent(ballContainer);

        isActive = false;

        //sprite.color = colorDefault;
    }
}
