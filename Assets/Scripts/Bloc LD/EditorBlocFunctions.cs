using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorBlocFunctions : MonoBehaviour
{
    public bool freeMove;
    public bool freeScale;
    private Vector3 previousPos;
    private Vector3 previousScale;
    private void LateUpdate()
    {
        if (!freeMove && previousPos != transform.position)
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

        if (!freeScale && previousScale != transform.localScale)
            transform.localScale = new Vector3(Mathf.RoundToInt(transform.localScale.x ), Mathf.RoundToInt(transform.localScale.y), 1);

        previousPos = transform.position;
        previousScale = transform.localScale;
    }

}
