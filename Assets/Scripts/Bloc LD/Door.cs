using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum doorTypes { KeyDoor, ButtonDoor};
    public KeyScript[] keys;
    private FMOD.Studio.EventInstance slidingSound;
    public Vector2 doorDestination;
    public float doorOpenSpeed = 1;
    public float doorCloseSpeed = 1;
    public AnimationCurve openCurve;
    public AnimationCurve closeCurve;
    int nOpen;
    private Vector2 ogPos;
    private IEnumerator doorOpening, doorClosing;
    LineRenderer line;
    public Color open, close;
    bool isOpen;
    private void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = keys.Length + 1;
        for (int i = 0; i < keys.Length; i++)
        {
            line.SetPosition(i, keys[i].transform.position);
        }
        line.SetPosition(keys.Length, transform.position);
        
        foreach(KeyScript key in keys)
        {
            key.door = this;
        }
        ogPos = transform.position;
        doorOpenSpeed = Mathf.Clamp(doorOpenSpeed, 0.01f, 100);
        doorCloseSpeed = Mathf.Clamp(doorCloseSpeed, 0.01f, 100);
        line.startColor = open;
        line.endColor = open;
    }

    private void Update()
    {
        line.SetPosition(keys.Length, transform.position);
        if (isOpen)
        {
            line.startColor = open;
            line.endColor = open;
        }
        else
        {
            line.startColor = close;
            line.endColor = close;
        }
    }
    public void KeyTriggered()
    {

        nOpen = 0;
        foreach (KeyScript key in keys)
        {
            if (key.activated == true) nOpen++;
        }
        if (nOpen == keys.Length)
        {
            if (doorClosing != null)
            {
                StopCoroutine(doorClosing);
                doorClosing = null;
            }
            if (doorOpening == null && (Vector2)transform.position != doorDestination)
            {
                doorOpening = DoorOpen(transform.position);
                StartCoroutine(doorOpening);
            }
        }
        else
        {
            if (doorOpening != null)
            {
                StopCoroutine(doorOpening);
                doorOpening = null;
            }
            if (doorClosing == null && (Vector2)transform.position != ogPos)
            {
                doorClosing = DoorClose(transform.position);
                StartCoroutine(doorClosing);
            }
        }
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    IEnumerator DoorOpen(Vector2 startPos)
    {
        float i = Vector2.Distance(ogPos, transform.position) / Vector2.Distance(ogPos, doorDestination) ;
        if (!IsPlaying(slidingSound))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/DoorOpen");
            slidingSound = FMODUnity.RuntimeManager.CreateInstance("event:/MouvementCorde/LineSound");
            slidingSound.start();
        }
        while (i < 1)
        {
            i += Time.deltaTime * doorOpenSpeed;
            transform.position = Vector2.Lerp(startPos, doorDestination, openCurve.Evaluate(i));
            isOpen = true;

            yield return null;
        }
        transform.position = doorDestination;
        isOpen = true;
        slidingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        slidingSound.release();
        FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/DoorClose");
        yield return null;
    }

    IEnumerator DoorClose(Vector2 startPos)
    {
        float i = Vector2.Distance(doorDestination, transform.position) / Vector2.Distance(ogPos, doorDestination);
        if (!IsPlaying(slidingSound))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/DoorOpen");
            slidingSound = FMODUnity.RuntimeManager.CreateInstance("event:/MouvementCorde/LineSound");
            slidingSound.start();
        }
        while (i < 1)
        {
            i += Time.deltaTime * doorOpenSpeed;
            transform.position = Vector2.Lerp(startPos, ogPos, openCurve.Evaluate(i));
            isOpen = false;

            yield return new WaitForEndOfFrame();
        }
        transform.position = ogPos;

        isOpen = false;
        slidingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        slidingSound.release();
        FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/DoorClose");
        yield return null;
    }
}

