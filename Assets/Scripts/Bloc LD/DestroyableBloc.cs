using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableBloc : MonoBehaviour
{
    public float necessaryVelocity;
    private void OnCollisionEnter2D(Collision2D collision)
    {
            print(collision.relativeVelocity.magnitude);
        if (collision.collider.CompareTag("Ball") && collision.relativeVelocity.magnitude > necessaryVelocity)
        {
            Destroy(gameObject);
        }
    }
}
