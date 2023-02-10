using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_SansGravite : Ball_State
{
    float realAngularDrag;
    float realBounciness;
    float realGravityScale;
    private void OnEnable()
    {
        //sprite.color = color;

        realGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;

        realAngularDrag = rb.angularDrag;
        rb.angularDrag = 0f;

        realBounciness = rb.sharedMaterial.bounciness;
        rb.sharedMaterial.bounciness = 1;
    }
    void Update()
    {
       //Comportement

       //Transition
    }
    private void OnDisable()
    {
        rb.gravityScale = realGravityScale;
        realGravityScale = 0f;

        rb.angularDrag = realAngularDrag;
        realAngularDrag = 0f;

        rb.sharedMaterial.bounciness = realBounciness;
        realBounciness = 0f;

        //sprite.color = colorDefault;
    }
}
