using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    private LineCreator lineC;
    //FMOD.Studio.EventInstance colInstance;
    Rigidbody2D rb;
    private void Awake()
    {
        lineC = GetComponent<LineCreator>();
        //colInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Ball/Collision");
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        lineC.LineUpdater();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("VolumeColBall", rb.velocity.magnitude);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Ball/Collision");
    }

}
