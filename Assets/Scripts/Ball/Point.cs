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
        RaycastHit2D hit = Physics2D.Raycast(pos, new Vector2(0, -1), Mathf.Infinity, LayerMask.GetMask("Ground"));
        if (timerOn && timerValue > 0)
        {
            timerValue -= Time.deltaTime;
        }
        else if (timerOn && timerValue <= 0 && (hit && Vector2.Distance(hit.point, pos) > 0))
        {
            Debug.Log(Vector2.Distance(hit.point, pos - (-Vector2.up)));
            Debug.Log(hit.collider);
            pos.y -= Time.deltaTime * descendSpeed;
        }
    }
}
