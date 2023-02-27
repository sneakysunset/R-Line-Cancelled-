using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { holdable, interactable};
    public ItemType itemType;
    public bool generateLine = false;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public ThrowPreview tP;
    [HideInInspector] public Collider2D col;
    [HideInInspector] public Transform heldPoint;
    [HideInInspector] public bool isHeld;
    public bool throwPreview;
    [HideInInspector] public GameObject Highlight;
    public bool catchable = true;
    private string mytag;
    protected Collider2D triggerBox;
    public virtual void Awake()
    {
        Highlight = transform.Find("Highlight").gameObject;
        if (TryGetComponent<Rigidbody2D>(out rb)) { }
        if (TryGetComponent<ThrowPreview>(out tP)) { }
        if(itemType == ItemType.holdable) col = transform.Find("Collider").GetComponent<Collider2D>();
        if (transform.Find("lineChecker")) 
            triggerBox = transform.Find("lineChecker").GetComponent<Collider2D>();
        mytag = tag;

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
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        setTagsLayers(mytag, mytag, 14);
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Catch");
        Physics2D.IgnoreCollision(player.coll, col, true);
        rb.isKinematic = true;
        if(player.holdableItems.Contains(this))
            player.holdableItems.Remove(this);
        heldPoint = holdPoint;
        Highlight.SetActive(false);
    }

    public virtual void GrabRelease(Player player)
    {
        isHeld = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        Physics2D.IgnoreCollision(player.coll, col, false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Grab");
        player.holdableItems.Add(this);
        setTagsLayers(mytag, mytag, 17);
    }

    public virtual void ThrowStarted(float throwStrength, Player player)
    {
        if(throwPreview)
        {
            tP.pointFolder.gameObject.SetActive(true);
            player.canMove = false;
            //player.canJump = false;
        }
    }

    public virtual void ThrowHeld(float throwStrength, Player player)
    {
        tP.Sim(throwStrength * player.moveValue, true);
    }

    public virtual void ThrowRelease(float throwStrength, Player player)
    {
        setTagsLayers(mytag, mytag, 17);
        rb.constraints = RigidbodyConstraints2D.None;
        tP.pointFolder.gameObject.SetActive(false);
        tP._line.positionCount = 1;
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");
        player.canMove = true;
        player.canJump = true;
        rb.isKinematic = false;
        player.holdableItems.Add(this);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("LineCollider") && isHeld)
        {
            Physics2D.IgnoreCollision(collision, col, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("LineCollider"))
        {
            Physics2D.IgnoreCollision(collision, col, false);
        }
    }
}
