using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPath : MonoBehaviour
{
    public int maxBounceNum = 2;
    public LayerMask bounceMask;
    public LayerMask hitLayer;
    LineRenderer lineR;
    Collider2D tempHit;
    private void Start()
    {
        lineR = GetComponentInChildren<LineRenderer>();
        lineR.positionCount = maxBounceNum;        
    }

    private void Update()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.right;

        

        for (int i = 0; i < maxBounceNum; i++)
        {
            print(i + " " + direction);
            Physics2D.queriesHitTriggers = false;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, Mathf.Infinity, hitLayer);
            Physics2D.queriesHitTriggers = true;
            Debug.DrawLine(origin, hit.point, Color.red);
            if (hit.collider != null)
            {
                var angle = Vector2.Angle(origin, hit.normal);
                direction = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
                origin = (Vector2)origin - hit.point * .9f;
                lineR.SetPosition(i, hit.point);
                Debug.DrawRay(origin, direction, Color.blue);
            }
            else break;
        }

    }

    private void OnDrawGizmos()
    {
        
    }
}
