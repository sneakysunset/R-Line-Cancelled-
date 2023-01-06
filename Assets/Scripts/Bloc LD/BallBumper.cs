using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBumper : MonoBehaviour
{
    public float bumperStrength;
    public enum bumperTypes { DirectionalBumper, NormalBumper };
    public enum bumperBounceTypes { BallVelocity, FlatVelocity };
    public bumperTypes bumperType;
    public bumperBounceTypes bumperBounceType;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
            float velocityH = 0;
            if (bumperBounceType == bumperBounceTypes.BallVelocity) velocityH = collision.relativeVelocity.magnitude * (bumperStrength / 10);
            else if(bumperBounceType== bumperBounceTypes.FlatVelocity) velocityH = bumperStrength;

            if (bumperType == bumperTypes.DirectionalBumper) collision.rigidbody.AddForce(transform.up * velocityH, ForceMode2D.Impulse);
            else if (bumperType == bumperTypes.NormalBumper) collision.rigidbody.AddForce(-collision.contacts[0].normal * velocityH, ForceMode2D.Impulse);
        }
    }
}
