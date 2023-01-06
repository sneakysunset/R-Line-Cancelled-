using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Absorption : MonoBehaviour
{
    ConstantMeshGeneration meshG;
    CharacterController2D charC;
    [Header("Inputs")]
    [Space(5)]
    public KeyCode absorbLineKey;

    [Space(10)]
    [Header("Absorb Variables")]
    [Space(5)]
    public int numberOfPointAbsorbed;
    public float absorbRate;
    bool absorbing;
    bool absorbFlag = false;

    private void Start()
    {
        meshG = GetComponent<ConstantMeshGeneration>();
        charC = GetComponent<CharacterController2D>();
    }

    private void Update()
    {
        if (Input.GetKey(absorbLineKey))
        {
            absorbing = true;
        }
        else absorbing = false;        
    }

    private void FixedUpdate()
    {
        if (absorbing && meshG.pointList.Count > 1 && !absorbFlag)
        {
            StartCoroutine(Utils_Mesh.MeshAbsorption(absorbRate, numberOfPointAbsorbed, meshG.pointList, charC, () => { absorbFlag = false; meshG.MeshCreator(); }));
            absorbFlag = true;
        }
        else
        {
            meshG.MeshCreator();
        }
    }
}
