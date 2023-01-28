using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { holdable, interactable};
    public ItemType itemType;
    public bool generateLine = false;
    protected Rigidbody2D rb;
    protected ThrowPreview tP;
    protected Collider2D col;
    private Transform heldPoint;
    protected bool isHeld;
    public bool throwPreview;
    [HideInInspector] public GameObject Highlight;

    public virtual void Start()
    {
        Highlight = transform.Find("Highlight").gameObject;
        if (TryGetComponent<Rigidbody2D>(out rb)) { }
        if (TryGetComponent<ThrowPreview>(out tP)) { }
        if (generateLine) 
        col = GetComponentInChildren<Collider2D>();
    }

    public virtual void OnEnable()
    {
        
    }

    public virtual void Update()
    {
        if (isHeld)
        {
            transform.position = heldPoint.position;
        }
    }

    public virtual void InteractStarted()
    {

    }

    public virtual void GrabStarted(Transform holdPoint)
    {
        isHeld = true;
        col.gameObject.layer = 14;
        tag = "Held";
        col.tag = "Held";
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Catch");
        rb.isKinematic = true;
        heldPoint = holdPoint;
    }

    public virtual void GrabRelease()
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Grab");

        tag = "NotHeld";
        col.tag = "NotHeld";
        col.gameObject.layer = 17;
    }

    public virtual void ThrowStarted(float throwStrength, CharacterController2D charC)
    {
        if(throwPreview)
        {
            tP.pointFolder.gameObject.SetActive(true);
            charC.canMove = false;
        }
    }

    public virtual void ThrowHeld(float throwStrength, CharacterController2D charC)
    {
        tP.Sim(throwStrength * charC.moveValue);
    }

    public virtual void ThrowRelease(float throwStrength, CharacterController2D charC)
    {
        charC.canMove = true;
        tP.pointFolder.gameObject.SetActive(false);
        tag = "NotHeld";
        col.tag = "NotHeld";
        col.gameObject.layer = 17;
        rb.isKinematic = false;
        tP._line.positionCount = 1;
        rb.velocity = charC.moveValue * throwStrength;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");
        isHeld = false;
    }

    public virtual void CancelThrow()
    {
        tP.pointFolder.gameObject.SetActive(false);
    }

    public virtual void SecondaryInputStarted()
    {

    }

    public virtual void SecondaryInputHeld()
    {

    }

    public virtual void SecondaryInputReleased()
    {

    }
}
