using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InputReceptor : Trigger
{
    public Trigger[] triggers;
    
    private void Update()
    {
        CheckTriggers();
    }

    private void CheckTriggers()
    {
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
