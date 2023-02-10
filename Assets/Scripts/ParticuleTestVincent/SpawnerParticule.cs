using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerParticule: MonoBehaviour
{
    public GameObject particulePrefab;
    [Range(0,5000)] public int maxParticule = 20;
    [Range(1, 5)] public int particulePerFrame;
    public int particuleCurrently = 0;
    Transform particuleContainer;

    private void Start()
    {
        particuleContainer = GameObject.Find("ParticuleContainer").transform;
        InitialPopulation();
    }
    private void Update()
    {
        //MaintainPop();
    }
    void InitialPopulation()
    {
        for (int i = 0; i < maxParticule; i++)
        {
            InstantiateParticule(CreateRandomPos());
        }
    }
    void InstantiateParticule(Vector2 pos)
    {
        particuleCurrently++;
        Vector3 realPos = new Vector3(pos.x, pos.y, 0);
        Instantiate(particulePrefab, realPos, Quaternion.identity,particuleContainer);
    }
    Vector2 CreateRandomPos()
    {
        Vector2 pos = new Vector2(Random.Range(transform.position.x - (transform.localScale.x / 2), transform.position.x + (transform.localScale.x / 2)), Random.Range(transform.position.y - (transform.localScale.y / 2), transform.position.y + (transform.localScale.y / 2)));
        return pos;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
    void MaintainPop()
    {
        if (particuleCurrently < maxParticule)
        {
            for (int i = 0; i < particulePerFrame; i++)
            {
                InstantiateParticule(CreateRandomPos());
            }
        }
    }
}
