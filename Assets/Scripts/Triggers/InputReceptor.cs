 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InputReceptor : Trigger
{
    public Trigger[] triggers;
    public int numberOfActivated;
    private void Update()
    {
        CheckTriggers();
    }

    private void CheckTriggers()
    {
        numberOfActivated = 0;
        foreach (Trigger trigger in triggers)
        {
            if (!trigger.activated)
            {
                if (activated)
                {
                    OnKeyDesactivationEvent?.Invoke();
                    activated = false;
                } 
                return;
            }
        }
        if (!activated)
        {
            OnKeyActivationEvent?.Invoke();
            activated = true;
        }
    }
}
