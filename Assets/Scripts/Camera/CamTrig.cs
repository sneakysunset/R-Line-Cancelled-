using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTrig : MonoBehaviour
{
    public bool useTransformPos, useOldCamSize;
    public Vector2 endPosition;
    Vector3 endPos;
    Transform camC;
    public float camSizeDestination;
    public float speed = 1;
    public AnimationCurve animCurveCam, animCurveCamSize;
    public IEnumerator camTransEnum;
    Camera cam;


    private void Start()
    {
        if (speed == 0) Debug.LogError("Speed cannot be equal to zero" + "   " + this.name);
        cam = Camera.main;
        if (useOldCamSize) camSizeDestination = cam.orthographicSize;

        camC = cam.transform.parent;
        if (useTransformPos) endPos = new Vector3(transform.position.x, transform.position.y, -1);
        else endPos = new Vector3(endPosition.x, endPosition.y, -1);

        //if (useOldCamSize) camSizeDestination = cam.orthographicSize;
    }

    //Quand la balle rentre dans un nouvel écran lance la coroutine de lerp vers le prochain écran.
    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Player" && Time.time > 1)
        {
            if (camTransEnum != null)
            {
                StopCoroutine(camTransEnum);
            }
            camTransEnum = cameraLerp(camC.position, cam.orthographicSize);
            StartCoroutine(camTransEnum);
        }
    }
    
    //Lerp la position de la caméra de startPos vers la position du prochain écran.
    //Actuellement désactivé mais lerp aussi la taille de l'écran.
    IEnumerator cameraLerp(Vector3 startPos, float startCamSize)
    {
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * (1 / speed);

            camC.position = Vector3.Lerp(startPos, endPos, animCurveCam.Evaluate(i));
            //cam.orthographicSize = Mathf.Lerp(startCamSize, camSizeDestination, animCurveCamSize.Evaluate(i));
            yield return new WaitForEndOfFrame();
        }
        camC.position = endPos;
        //cam.orthographicSize = camSizeDestination;
        camTransEnum = null;
        yield return null;
    }
}
