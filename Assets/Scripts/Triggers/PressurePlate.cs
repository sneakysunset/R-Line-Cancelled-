using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : Trigger
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !activated)
        {
            OnKeyActivationEvent?.Invoke();
            activated = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && activated)
        {
            OnKeyDesactivationEvent?.Invoke();
            activated = false;
        }
    }
}
