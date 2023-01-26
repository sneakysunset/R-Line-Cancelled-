using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [TextArea]
    public string infoForceField = "Le champ de force pousse la balle dans le sens de la flèche. Vous pouvez la faire tourner sur l'axe Z mais aussi scale l'objet etc";
    private Rigidbody2D ballRB;
    [Tooltip("Force du champ de Force")]
    public float force = 3f;
    private Vector2 directionForce;

    private void Update()
    {
        directionForce = transform.TransformDirection(Vector2.down);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + directionForce * force);
    }

    //Récupère le script de la balle quand elle rentre dans la zone de champ de force
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            ballRB = collision.GetComponentInParent<Rigidbody2D>();
        }
    }

    //Applique une force à la balle correspondant à la variable "force" dans la direction transform.down du champ de force.
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {     
            ballRB.velocity += directionForce * force;
        }
       
    }

}
