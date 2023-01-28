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
        col.gameObject.layer = 14;
    }

    public override void ThrowRelease(float throwStrength, CharacterController2D charC)
    {
        base.ThrowRelease(throwStrength, charC);
        tag = "Ball";
        col.tag = "Ball";
        col.gameObject.layer = 7;
        col.gameObject.layer = 14;
    }
}
