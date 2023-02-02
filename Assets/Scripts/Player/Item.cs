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
    [HideInInspector] public bool isHeld;
    public bool throwPreview;
    [HideInInspector] public GameObject Highlight;

    public virtual void Start()
    {
        Highlight = transform.Find("Highlight").gameObject;
        if (TryGetComponent<Rigidbody2D>(out rb)) { }
        if (TryGetComponent<ThrowPreview>(out tP)) { }
        col = GetComponentInChildren<Collider2D>();
    }

    public virtual void OnEnable()
    {
        
    }

    public virtual void Update()
    {

    }

    public virtual void setTagsLayers(string colTag, string Tag, int colLayer)
    {
        col.tag = colTag;
        tag = Tag;
        col.gameObject.layer = colLayer;
    }

    public virtual void FixedUpdate()
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
        setTagsLayers("Held", "Held", 14);
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Catch");
        rb.isKinematic = true;
        heldPoint = holdPoint;
        Highlight.SetActive(false);
    }

    public virtual void GrabRelease()
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Grab");

        setTagsLayers("NotHeld", "NotHeld", 17);
    }

    public virtual void ThrowStarted(float throwStrength, CharacterController2D charC, ItemSystem iS)
    {
        if(throwPreview)
        {
            tP.pointFolder.gameObject.SetActive(true);
            charC.canMove = false;
            charC.canJump = false;
        }
    }

    public virtual void ThrowHeld(float throwStrength, CharacterController2D charC)
    {
        tP.Sim(throwStrength * charC.moveValue);
    }

    public virtual void ThrowRelease(float throwStrength, CharacterController2D charC)
    {
        setTagsLayers("NotHeld", "NotHeld", 17);

        tP.pointFolder.gameObject.SetActive(false);
        tP._line.positionCount = 1;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");
        charC.canMove = true;
        charC.canJump = true;
        rb.isKinematic = false;
        rb.velocity = charC.moveValue * throwStrength;

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
