 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InputReceptor : Trigger
{
    public Trigger[] triggers;
    int numberOfActivated;


    private void Update()
    {
        CheckTriggers();
    }

    private void CheckTriggers()
    {
        numberOfActivated = 0;
        foreach (Trigger trigger in triggers)
        {
            if (trigger.activated) numberOfActivated++;
            else
            {
                if (activated)
                {
                    OnKeyDesactivationEvent?.Invoke();
                    activated = false;
                }
                return;
            }
        }


        if (numberOfActivated == triggers.Length)
        {
            OnKeyActivationEvent?.Invoke();
            activated = true;
        }
    }
}
