using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Player_CollisionEventHandler : MonoBehaviour
{
    public UnityEvent<Collision2D> ColEnterEvent;
    public UnityEvent<Collision2D> ColStayEvent;
    public UnityEvent<Collision2D> ColExitEvent;
    public UnityEvent<Collider2D> TrigEnterEvent;
    public UnityEvent<Collider2D> TrigStayEvent;
    public UnityEvent<Collider2D> TrigExitEvent;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ColEnterEvent?.Invoke(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ColStayEvent?.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        ColExitEvent?.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TrigEnterEvent?.Invoke(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TrigStayEvent?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TrigExitEvent?.Invoke(collision);
    }

}
