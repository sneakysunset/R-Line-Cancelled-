using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    private LineCreator lineC;
    Rigidbody2D rb;

    private void Awake()
    {
        lineC = GetComponent<LineCreator>();
        rb = GetComponent<Rigidbody2D>();
    }

    //Lance la logique de la ligne.
    private void FixedUpdate()
    {
        lineC.LineUpdater();
    }

    //Son de collision de la balle.
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

}
