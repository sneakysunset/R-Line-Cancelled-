using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Ball : Item
{
    protected LineCreator lC;

    public override void Start()
    {
        base.Start();
        lC = GetComponent<LineCreator>();
    }

    public override void GrabRelease()
    {
        base.GrabRelease();
        tag = "Ball";
        col.tag = "Ball";
        col.gameObject.layer = 7;
    }

    public override void ThrowRelease(float throwStrength, CharacterController2D charC)
    {
        base.ThrowRelease(throwStrength, charC);
        tag = "Ball";
        col.tag = "Ball";
        col.gameObject.layer = 7;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        lC.LineUpdater();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Le paramètre global "VolumeColBall" sert à changer le volume de l'event de collision de balle. Sa valeur est déterminé par la vitesse de la balle au moment de l'impact.
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("VolumeColBall", rb.velocity.magnitude);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Ball/Collision");
    }
}
