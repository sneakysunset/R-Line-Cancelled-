using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfParticule : MonoBehaviour
{
    SpawnerParticule spawner;
    private void Start()
    {
        spawner = GetComponent<SpawnerParticule>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Particule"))
        {
            Destroy(collision.gameObject);
            spawner.particuleCurrently--;
        }
    }
}
