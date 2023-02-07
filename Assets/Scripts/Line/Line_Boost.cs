using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Boost : MonoBehaviour
{
    public float speedBoost;
    public float boostDuration = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            print(collision.name);

            StartCoroutine(booster(collision.attachedRigidbody));
        }
    }

    IEnumerator booster(Rigidbody2D rb)
    {
        float i = 0;
        while(i < 1)
        {
            rb.velocity *= speedBoost;
            i += Time.deltaTime * (1 / boostDuration);
            yield return new WaitForEndOfFrame();
        }
    }
}
