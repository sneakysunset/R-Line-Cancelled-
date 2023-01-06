using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

public class LineBallSpawner : MonoBehaviour
{
    [TextArea]
    public string infoSpawner = "En appuyant sur P tu fais respawn la ball,NE PAS METTRE PLUSIEURS SPAWNER POUR LE MOMENT";
    public GameObject currentBall;
    public GameObject lineBallPrefab;
    public Transform spawnPoint;
    public float throwStrength;
    public bool spawnOnStart;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) Spawn();
    }

    private void Start()
    {
        if (spawnOnStart) Spawn();
    }

    public void Spawn()
    {
        if (currentBall)
        {
            Destroy(currentBall);
        }
        currentBall = Instantiate(lineBallPrefab, spawnPoint.position, transform.rotation);
        currentBall.GetComponent<Rigidbody2D>().AddForce(-transform.up * throwStrength, ForceMode2D.Impulse);
    }
}
