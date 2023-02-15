using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public Vector2 pos;
    public bool timerOn;
    public float timerValue;
    private float descendSpeed;
    private float descendSpeedAccel;
    private float f;
    Vector2 hit;

    public Point(Vector2 pos)
    {
        this.pos = pos;
    }

    public void TimerTrigger(bool timerOn, float _timerValue, float _descendSpeed, float _descendSpeedAccel, float width, bool cascadeLowToGround)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(pos, -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Ground"));
        if (hit2D)
        {
            hit = hit2D.point + (Vector2.up * (width/2));
        }
        else Debug.LogError("NoHit");
        this.timerOn = timerOn;
        descendSpeedAccel = _descendSpeedAccel;
        descendSpeed = _descendSpeed;
        timerValue = _timerValue;
        if (Vector2.Distance(hit, pos) < 1 && hit.y < pos.y && cascadeLowToGround) this.timerOn = true;   

        f = 0;
    }

    public void Printer()
    {
        Debug.Log("Distance : " + Vector2.Distance(hit, pos));
    }

    public void Fond()
    {
        if (timerOn && timerValue > 0)
        {
            timerValue -= Time.deltaTime;

        }
        else if ((timerOn && timerValue <= 0 && (Vector2.Distance(hit, pos) > 0) && hit.y < pos.y) )
        {
            linearFall();
        }
        else if ((timerOn && timerValue <= 0 && hit.y > pos.y) )
        {
            pos.y = hit.y ;
            timerOn = false;
        }
    }

    void linearFall()
    {
        descendSpeedAccel += Time.deltaTime * 10;
        descendSpeed = Time.deltaTime * descendSpeedAccel;
        pos.y -= descendSpeed;
    }
}
