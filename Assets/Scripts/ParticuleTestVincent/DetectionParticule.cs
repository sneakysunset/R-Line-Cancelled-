using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionParticule : MonoBehaviour
{
    [Range(1f,10f)]public float rangeDetection = 5f;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeDetection);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Particule"))
        {
            Destroy(collision.gameObject);
        }
    }
}
