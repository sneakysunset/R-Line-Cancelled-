using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    Vector2 medianPos;
    Transform P1, P2;
    Vector3 velocity;
    public float smoother;
    public float cameraExpandSpeed = 1;
    public float maximumDezoom = 0;
    public float minimumDezoom = -6.5f;
    [SerializeField] private float distanceX, distanceY;
    float recul;
    private void Start()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        P1 = players[0].transform;
        P2 = players[1].transform;
    }

    private void Update()
    {
        distanceX = Mathf.Abs(P1.position.x - P2.position.x);
        distanceY = Mathf.Abs(P1.position.y - P2.position.y);
        recul = Mathf.Clamp((distanceX - 5) * cameraExpandSpeed + distanceY * cameraExpandSpeed, 10, 20);
        //recul = Mathf.Clamp((distanceX - 17) * cameraExpandSpeed, 0, 6.5f) + Mathf.Clamp(distanceY * cameraExpandSpeed, 0f, 25);
        medianPos = new Vector2((P1.position.x + P2.position.x) / 2, (P1.position.y + P2.position.y) / 2);
    }

    private void LateUpdate()
    {
        Vector3 temp = Vector3.SmoothDamp(transform.position, medianPos, ref velocity, smoother);
        //temp.z = recul;
        Camera.main.orthographicSize = recul;
        transform.position = temp;
    }
}
