using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Door : MonoBehaviour
{
    #region variables
    private FMOD.Studio.EventInstance slidingSound;
    [Range(.01f, 100f)] public float doorOpenSpeed = 1;
    [Range(.01f, 100f)] public float doorCloseSpeed = 1;
    public AnimationCurve openCurve;
    public AnimationCurve closeCurve;
    int nOpen;
    private Vector2 ogPos;
    private IEnumerator doorOpening, doorClosing;
    private Rigidbody2D rb;
    [HideInInspector] public bool isOpen;
    [HideInInspector] public Vector3 prevPos;
    bool isBlocked;
    #endregion

    #region Functions
    private void Start()
    {
        ogPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    public void OpenDoor()
    {
        if (doorClosing != null)
        {
            StopCoroutine(doorClosing);
            doorClosing = null;
        }
        if (doorOpening == null && (Vector2)transform.position != (Vector2)destination)
        {
            doorOpening = DoorOpen(transform.position);
            StartCoroutine(doorOpening);
        }
    }

    public void CloseDoor()
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


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("LineCollider"))
        {
            if(doorOpening != null)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.position - destination, Mathf.Infinity, 6);
                if (hit.collider == collision)
                    isBlocked = true;
                else isBlocked = false;
            }
            else if(doorClosing != null)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.position - (Vector3)ogPos, Mathf.Infinity, 6);
                if (hit.collider == collision)
                    isBlocked = true;
                else isBlocked = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("LineCollider"))
        {
            if (doorOpening != null || doorClosing != null)
            {
                isBlocked = false;
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
        float i = Vector2.Distance(ogPos, transform.position) / Vector2.Distance(ogPos, destination) ;
        if (!IsPlaying(slidingSound))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/DoorOpen");
            slidingSound = FMODUnity.RuntimeManager.CreateInstance("event:/MouvementCorde/LineSound");
            slidingSound.start();
        }
        while (i < 1)
        {
            i += Time.deltaTime * doorOpenSpeed;
            transform.position = Vector2.Lerp(startPos, destination, openCurve.Evaluate(i));
            isOpen = true;
            if (isBlocked) print(1);
            yield return new WaitUntil(() => !isBlocked);

            yield return new WaitForEndOfFrame();
        }
        transform.position = destination;
        isOpen = true;
        doorOpening = null;
        slidingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        slidingSound.release();
        FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/DoorClose");
        yield return null;
    }

    IEnumerator DoorClose(Vector2 startPos)
    {
        float i = Vector2.Distance(destination, transform.position) / Vector2.Distance(ogPos, destination);
        if (!IsPlaying(slidingSound))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/DoorOpen");
            slidingSound = FMODUnity.RuntimeManager.CreateInstance("event:/MouvementCorde/LineSound");
            slidingSound.start();
        }
        while (i < 1)
        {
            i += Time.deltaTime * doorCloseSpeed;
            transform.position = Vector2.Lerp(startPos, ogPos, closeCurve.Evaluate(i));
            isOpen = false;

            yield return new WaitUntil(() => !isBlocked);
            yield return new WaitForEndOfFrame();
        }
        transform.position = ogPos;

        isOpen = false;
        doorClosing = null;
        slidingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        slidingSound.release();
        FMODUnity.RuntimeManager.PlayOneShot("event:/BlockLd/DoorClose");
        yield return null;
    }
    #endregion

    #region Editor Functions
    [SerializeField, HideInInspector] public Vector3 destination;

    public void MovePoint(bool pA, Vector3 pos, Vector3 center)
    {
            var a = destination;
            destination = new Vector3(pos.x, pos.y, 0);
    }

    private void OnDrawGizmos()
    {
        if (Selection.activeGameObject != gameObject)
        {
            Gizmos.DrawWireSphere(destination, .4f);
        }
    }
    #endregion
}


#if UNITY_EDITOR
[CustomEditor(typeof(Door))]
public class Door_Editor : Editor
{
    Door door;

    private void OnSceneGUI()
    {
        Draw();
    }

    void Draw()
    {
        MoveHandlesAlongObject();
        Handles.color = Color.black;
        Handles.DrawLine(door.destination, door.transform.position);

        Handles.color = Color.red;
        Vector3 newPosA = Handles.FreeMoveHandle(door.destination, Quaternion.identity, .5f, Vector3.zero, Handles.CylinderHandleCap);
        if (door.destination != newPosA)
        {
            Undo.RecordObject(door, "MovePoint");
            door.MovePoint(true, newPosA, door.transform.position);
        }
    }

    private void OnEnable()
    {
        door = (Door)target;
    }

    void MoveHandlesAlongObject()
    {
        if (door.prevPos != door.transform.position && !Application.isPlaying)
        {
            Vector3 movement = door.transform.position - door.prevPos;
            door.destination += movement;
            door.prevPos = door.transform.position;
        }
    }
}

#endif
