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
    public bool _timer;
    public float timerOff, timerOn;
    private float timer;
    bool isOn;
    public bool activated;
    Collider2D receptor;
    private void Start()
    {
        vector2s = new List<Vector2>();
        lineR = GetComponentInChildren<LineRenderer>();
        timer = timerOff;
        myCol = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (activated && _timer) LaserTimer();
        else if( !activated && _timer)
        {
            isOn = false;
            timer = timerOff;
        }
    }

    private void FixedUpdate()
    {
        if ((isOn || !_timer) && activated)
        {
            edgeC.enabled = true;
            Raycast();
        }
        else edgeC.enabled = false;
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
        if (!activated) return;
        origin = transform.position;
        Vector3 direction = transform.right;
        vector2s.Clear();
        vector2s.Add(origin);

        //myCol.isTrigger = true;
        for (int i = 0; i < maxBounceNum; i++)
        {
            Physics2D.queriesHitTriggers = false;
            myCol.isTrigger = true;
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, Mathf.Infinity);
            Physics2D.queriesHitTriggers = true;
            RaycastHit2D hit = hits[0];
            float distance = Mathf.Infinity;
            foreach(RaycastHit2D hitR in hits)
            {
                float d = Vector2.Distance(hitR.point, origin);
                if (d > .5f && d < distance)
                {
                    distance = d;
                    hit = hitR;
                }
            }

            if (hit.collider.CompareTag("LaserReceptor") && receptor == null)
            {
                receptor = hit.collider;
                Trigger trig = hit.transform.GetComponent<Trigger>();
                trig.activated = true;
                trig.OnKeyActivationEvent?.Invoke();
            }
            else if (!hit.collider.CompareTag("LaserReceptor") && receptor != null)
            {
                Trigger trig = receptor.GetComponent<Trigger>();
                trig.activated = false;
                trig.OnKeyDesactivationEvent?.Invoke();
                receptor = null;
            }

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
        vector2s.Add(hit.point);

        return direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && activated) Destroy(collision.transform.parent.gameObject);
    }

    public void ActivateLaser(bool activate)
    {
        if (!activate)
        {
            vector2s.Clear();
            Vector3[] vec = new Vector3[vector2s.Count];
            Vector2[] vec2d = new Vector2[vector2s.Count];
            lineR.SetPositions(vec);
            edgeC.points = new Vector2[vec2d.Length];
            edgeC.enabled = false;
            lineR.enabled = false;
        }
        else
        {
            edgeC.enabled = true;
            lineR.enabled = true;
        }
        activated = activate;
    }
}
