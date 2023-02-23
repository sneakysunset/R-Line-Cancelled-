using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float bumperStrength;
    //DirectionalBumper fait rebondir la balle dans la direction du bumper (transform.up).
    //NormalBumper fait rebondir la balle dans la direction de la normal de collision entre la balle et le bumper.
    public enum bumperTargets { Ball, Player, Everything}
    public enum bumperTypes { DirectionalBumper, NormalBumper };
    public enum bumperBounceTypes { BallVelocity, FlatVelocity };
    //BallVelocity utilise la vitesse de la balle actuelle pour la faire rebondir.
    //FlatVelocity fait rebondir la balle avec une vitesse constante.
    public bumperTypes bumperType;
    public bumperBounceTypes bumperBounceType;
    public bumperTargets bumperTarget;

    //Lors de la collision avec la balle lui confère une vitesse dans une direction dépendant des énumérateurs choisis au-dessus.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (bumperTarget)
        {
            case bumperTargets.Ball:
                if (collision.gameObject.tag == "Ball")
                {
                    Bump(collision);
                }
                break;
            case bumperTargets.Player:
                if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerOff")
                {
                    Bump(collision);
                }
                break;
            case bumperTargets.Everything:
                Bump(collision);
                break;
        }

    }

    void Bump(Collision2D collision)
    {
        float velocityH = 0;
        if (bumperBounceType == bumperBounceTypes.BallVelocity) velocityH = collision.relativeVelocity.magnitude * (bumperStrength / 10);
        else if (bumperBounceType == bumperBounceTypes.FlatVelocity) velocityH = bumperStrength;

        if (bumperType == bumperTypes.DirectionalBumper) collision.rigidbody.AddForce(transform.up * velocityH, ForceMode2D.Impulse);
        else if (bumperType == bumperTypes.NormalBumper) collision.rigidbody.AddForce(-collision.contacts[0].normal * velocityH, ForceMode2D.Impulse);
    }
}
