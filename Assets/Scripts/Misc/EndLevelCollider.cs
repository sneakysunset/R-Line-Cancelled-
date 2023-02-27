using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelCollider : MonoBehaviour
{
    private RoundManager rMan;
    void Start()
    {
        rMan = FindObjectOfType<RoundManager>();
    }

    void Update()
    {
            
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && rMan.roundEnded)
        {
            Time.timeScale = 0;
            Debug.Log("Winner : " + collision.transform.parent.name);
        }
    }
}
