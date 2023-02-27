using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [TextArea]
    public string infoForceField = "Le champ de force pousse la balle dans le sens de la flèche. Vous pouvez la faire tourner sur l'axe Z mais aussi scale l'objet etc";
    private List<Rigidbody2D> rbs;
    public enum targetType {Player, Ball, Everything };
    public targetType type;
    [Tooltip("Force du champ de Force")]
    public float force = 3f;
    private Vector2 directionForce;

    private void Start()
    {
        rbs = new List<Rigidbody2D>();
    }

    private void Update()
    {
        directionForce = -transform.up;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + directionForce * force);
    }

    //Récupère le script de la balle quand elle rentre dans la zone de champ de force
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (type)
        {
            case targetType.Player:
                if (collision.transform.root.CompareTag("Player"))
                {
                    rbs.Add (collision.GetComponentInParent<Rigidbody2D>());
                }
                break;
                case targetType.Ball:
                if (collision.CompareTag("Ball"))
                {
                    rbs.Add(collision.GetComponentInParent<Rigidbody2D>());
                }
                break;
                case targetType.Everything:
                rbs.Add(collision.GetComponentInParent<Rigidbody2D>());
                break;
        }
    }

    private void FixedUpdate()
    {
        if(rbs.Count > 0)
        {
             foreach (var b in rbs)
             {
                 b.velocity += directionForce * force;
             }
        }
    }

    //Applique une force à la balle correspondant à la variable "force" dans la direction transform.down du champ de force.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(rbs.Contains(collision.attachedRigidbody)) rbs.Remove(collision.attachedRigidbody);
    }

}
