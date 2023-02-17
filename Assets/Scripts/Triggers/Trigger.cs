using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent OnKeyActivationEvent;
    public UnityEvent OnKeyDesactivationEvent;
     public bool activated;
}
