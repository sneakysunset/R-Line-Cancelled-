using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum doorTypes { KeyDoor, ButtonDoor};
    public KeyScript[] keys;
    private FMOD.Studio.EventInstance slidingSound;
    public Vector2 doorDestination;
    [Range(.01f, 100f)] public float doorOpenSpeed = 1;
    [Range(.01f, 100f)] public float doorCloseSpeed = 1;
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

        //Génère une ligne reliant les clés et la porte.
        line.positionCount = keys.Length + 1;
        for (int i = 0; i < keys.Length; i++)
        {
            line.SetPosition(i, keys[i].transform.position);
        }
        line.SetPosition(keys.Length, transform.position);
        
        //Donne une référence à ce script aux interrupteurs associés.
        foreach(KeyScript key in keys)
        {
            key.door = this;
        }
        ogPos = transform.position;

        line.startColor = open;
        line.endColor = open;
    }

    private void Update()
    {
        //Actualise la position des points de la ligne reliant les clés et la porte.
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

    //A chaque fois qu'une clé est activée ou désactivée, si la porte est fermé check si toutes les clés sont activés dans le cas ou elles le sont toutes lance l'ouverture de la porte.
    //Sinon lance la fermeture de la porte.
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

    //Méthode cervant à vérifier si une instance Fmod donné est entrain de jouer.
    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    //Animation d'ouverture de la porte avec les sons d'ouverture de fermeture et de coulissement joués.
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
    //Animation de fermeture de la porte avec les sons d'ouverture de fermeture et de coulissement joués.
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

