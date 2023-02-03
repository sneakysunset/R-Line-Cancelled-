using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point 
{
    public Vector2 pos;
    public bool timerOn;
    public float timerValue;

    public Point(Vector2 pos)
    {
        this.pos = pos;
    }

    public void TimerTrigger(bool timerOn, float timerValue)
    {
        this.timerOn = timerOn;
        if(timerOn)
        {
            this.timerValue = timerValue;
        }
    }

    public void Fond(float descendSpeed)
    {
        if(timerOn && timerValue > 0)
        {
            timerValue -= Time.deltaTime;
        }
        else
        {
            pos.y -= Time.deltaTime * descendSpeed;
        }
    }
}
