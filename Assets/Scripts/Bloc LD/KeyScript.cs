using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public bool activated;
    [HideInInspector] public Door door;
    Color ogCol;

    private void Start()
    {
        ogCol = GetComponent<SpriteRenderer>().color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LineCollider"))
        {
            activated = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/SwitchOn");
            door.KeyTriggered();
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("LineCollider"))
        {
            activated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("LineCollider"))
        {
            activated = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/SwitchOff");

            door.KeyTriggered();
            GetComponent<SpriteRenderer>().color = ogCol;
        }
    }
}
