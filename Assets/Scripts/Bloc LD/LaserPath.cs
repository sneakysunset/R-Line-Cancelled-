using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPath : MonoBehaviour
{
    public int maxBounceNum = 2;
    public LayerMask bounceMask;
    public LayerMask hitLayer;
    LineRenderer lineR;
    List<Vector2> vector2s;
    Vector3 origin;
    Collider2D col;
    Collider2D myCol;
    public EdgeCollider2D edgeC;

    public float timerOff, timerOn;
    private float timer;
    bool isOn;
    public bool activated;

    private void Start()
    {
        vector2s = new List<Vector2>();
        lineR = GetComponentInChildren<LineRenderer>();
        timer = timerOff;
        myCol = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (activated) LaserTimer();
        else
        {
            isOn = false;
            timer = timerOff;
        }
    }

    private void FixedUpdate()
    {
        if(isOn) Raycast();
    }

    void LaserTimer()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && isOn)
        {
            isOn = false;
            timer = timerOff;
            lineR.positionCount = 0;
            edgeC.enabled = false;
        }
        else if (timer <= 0 && !isOn)
        {
            isOn = true;
            timer = timerOn;
        }
    }

    void Raycast()
    {
        origin = transform.position;
        Vector3 direction = transform.right;
        vector2s.Clear();
        vector2s.Add(origin);

        myCol.isTrigger = true;
        for (int i = 0; i < maxBounceNum; i++)
        {
            Physics2D.queriesHitTriggers = false;
            
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, Mathf.Infinity);
            Physics2D.queriesHitTriggers = true;

            if (hit.collider != null && hit.transform.gameObject.tag == "LineCollider")
            {
                direction = Bouncing(direction, hit);
            }
            else  if(hit.collider == null)
            {
                vector2s.Add(origin + direction * 100);
                break;
            }
            else
            {
                vector2s.Add(hit.point);
                break;
            }
            myCol.isTrigger = false;
        }

        if (col != null) col.isTrigger = false;
        col = null;
        Vector3[] vec = new Vector3[vector2s.Count];
        Vector2[] vec2d = new Vector2[vector2s.Count];

        for (int i = 0; i < vec.Length; i++)
        {
            vec[i] = vector2s[i];
            vec2d[i] = vector2s[i];
        }
        lineR.positionCount = vec.Length;
        lineR.SetPositions(vec);
        edgeC.points = new Vector2[vec2d.Length];
        edgeC.SetPoints(vector2s);
    }

    Vector3 Bouncing(Vector3 direction, RaycastHit2D hit)
    {
        direction = Vector2.Reflect(direction, hit.normal);
        origin = hit.point;
        if (col == null) col = hit.collider;
        col.isTrigger = false;
        col = hit.collider;
        col.isTrigger = true;
        vector2s.Add(hit.point);

        return direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Destroy(collision.gameObject);
    }

    public void ActivateLaser(bool activate) => activated = activate;
}
