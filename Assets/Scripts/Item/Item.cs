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

    public virtual void GrabStarted(Transform holdPoint, Player player)
    {
        isHeld = true;
        setTagsLayers("Held", "Held", 14);
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Catch");
        Physics2D.IgnoreCollision(player.coll, col, true);
        rb.isKinematic = true;
        heldPoint = holdPoint;
        Highlight.SetActive(false);
    }

    public virtual void GrabRelease(Player player)
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        Physics2D.IgnoreCollision(player.coll, col, false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Grab");

        setTagsLayers("NotHeld", "NotHeld", 17);
    }

    public virtual void ThrowStarted(float throwStrength, Player player)
    {
        if(throwPreview)
        {
            tP.pointFolder.gameObject.SetActive(true);
            player.canMove = false;
            player.canJump = false;
        }
    }

    public virtual void ThrowHeld(float throwStrength, Player player)
    {
        tP.Sim(throwStrength * player.moveValue);
    }

    public virtual void ThrowRelease(float throwStrength, Player player)
    {
        setTagsLayers("NotHeld", "NotHeld", 17);

        tP.pointFolder.gameObject.SetActive(false);
        tP._line.positionCount = 1;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");
        player.canMove = true;
        player.canJump = true;
        rb.isKinematic = false;
        Physics2D.IgnoreCollision(player.coll, col, false) ;
        rb.velocity = player.moveValue * throwStrength;

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
