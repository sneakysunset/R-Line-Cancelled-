using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : Trigger
{
    public List<Collider2D> collisions;
    public bool ballOnly;
    private void Start()
    {
        collisions = new List<Collider2D>();
    }

    private void Update()
    {
        if (activated && collisions.Count == 0)
        {
            activated = false;
        }
        foreach (Collider2D col in collisions)
        {
            if(col == null)
            {
                collisions.Remove(col);
                break;
            }
            else if (col.isTrigger || col.gameObject.layer == 14)
            {
                collisions.Remove(col);
                break;
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(ballOnly && (collision.collider.CompareTag("Ball") || collision.collider.CompareTag("Held")))
        {
            if (collisions.Count == 0)
            {
                OnKeyActivationEvent?.Invoke();
                activated = true;
            }
            collisions.Add(collision.collider);
            return;
        }

        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Cube") || collision.collider.CompareTag("Ball") || collision.collider.CompareTag("Held"))
        {
            if(collisions.Count == 0)
            {
                OnKeyActivationEvent?.Invoke();
                activated = true;
            }
            collisions.Add(collision.collider);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Cube") || collision.CompareTag("Ball") || collision.CompareTag("Held"))
        {
            if (collisions.Count == 0)
            {
                OnKeyActivationEvent?.Invoke();
                activated = true;
            }
            collisions.Add(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Cube") || collision.collider.CompareTag("Ball") || collision.collider.CompareTag("BallHeld"))
        {
            if(collisions.Count == 1)
            {
                OnKeyDesactivationEvent?.Invoke();
                activated = false;
            }
                collisions.Remove(collision.collider);
        }
    }
}
