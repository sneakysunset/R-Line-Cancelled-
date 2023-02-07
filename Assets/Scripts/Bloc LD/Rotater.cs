using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public bool rotate;
    public float rotateSpeed;
    private float timer;
    public float rotateTimerValue;

    private void Start()
    {
        timer = rotateTimerValue;
    }

    void FixedUpdate()
    {
        if(rotateTimerValue > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = rotateTimerValue;
                if (rotate) rotate = false;
                else rotate = true;
            }
        }

        if (rotate)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }

    public void ActivateRotation(bool stopRotation , bool rotTimer, float rotationLength)
    {
        if (!stopRotation)
        {
            if (rotTimer)
            {
                timer = rotationLength;
            }
            rotate = true;
        }
        else rotate = false;
    }
}
