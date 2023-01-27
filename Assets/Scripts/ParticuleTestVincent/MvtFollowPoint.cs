using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MvtFollowPoint : MonoBehaviour
{
    DetectionParticule detectionManager;
    [Range(1f,10f)] public float speed;
    public Color detectedColor;
    bool gizmo;
    private void Awake()
    {
        detectionManager = GameObject.Find("LineBall (2)").GetComponent<DetectionParticule>();
    }
    private void OnEnable()
    {
        gizmo = true;
        GetComponent<SpriteRenderer>().color = detectedColor;
    }
    void Update()
    {
        //Comportement
        GoToTargetBall();
        //Transition
        float distanceToPoint = Vector3.Distance(detectionManager.gameObject.transform.position, transform.position);
        if (distanceToPoint >= detectionManager.rangeDetection)
        {
            GetComponent<MvtParticule>().enabled = true;
            enabled = false;
        }
    }
    void GoToTargetBall()
    {
        transform.position = Vector2.MoveTowards(transform.position, detectionManager.gameObject.transform.position, speed * Time.deltaTime);
    }
    private void OnDisable()
    {
        gizmo = false;
    }
    private void OnDrawGizmos()
    {
        if (gizmo)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, detectionManager.gameObject.transform.position);
        }
    }
}
