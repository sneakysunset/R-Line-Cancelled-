using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Button : Item
{
    Trigger myTrigger;
    public float timerValue = 1;
    float timer = 0;
    public override void Start()
    {
        base.Start();
        myTrigger = GetComponent<Trigger>();
    }

    public override void InteractStarted()
    {
        base.InteractStarted();
        myTrigger.activated = true;
        myTrigger.OnKeyActivationEvent?.Invoke();
        timer = timerValue;
    }

    public override void Update()
    {
        base.Update();
        if (myTrigger.activated) timer -= Time.deltaTime;

        if(myTrigger.activated && timer <= 0)
        {
            myTrigger.activated = false;
            myTrigger.OnKeyDesactivationEvent?.Invoke();
        }
    }
}
