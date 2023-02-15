using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : Trigger
{
    public List<Collider2D> collisions;

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
        foreach (Collider2D col in collisions) if (col.isTrigger || col.gameObject.layer == 14) collisions.Remove(col);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Cube"))
        {
            if(collisions.Count == 0)
            {
                OnKeyActivationEvent?.Invoke();
                activated = true;
            }
            collisions.Add(collision.collider);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Cube"))
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
