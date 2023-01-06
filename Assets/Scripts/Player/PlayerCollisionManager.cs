using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    [HideInInspector] public CharacterController2D charC;
    private Rigidbody2D rb;
    [HideInInspector] public GameObject coll;
    Vector2 prevVelocity;
    [HideInInspector] public IEnumerator groundCheckEnum;
    [HideInInspector] public List<Transform> holdableObjects;
    [HideInInspector] public bool holdingBall = false;

    [Range(-1f, 1f)] public float yGroundCheck = -.15f;
    [Range(-1f, 1f)] public float yWallJump = .7f;
    public float bounce = 1.5f;

    private void Start()
    {
        charC = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        coll = transform.Find("Collider").gameObject;
    }

    private void FixedUpdate()
    {
        StartCoroutine(WaitForPhysics());
    }

    bool enterAgain;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LineCollisionEnter(collision);
        GroundCheckCollisionEnter(collision);
        CollisionWithPlayer(collision);
        if (collision.contacts[0].normal.y > -yWallJump && collision.contacts[0].normal.y < yWallJump && collision.gameObject.CompareTag("Jumpable") /*|| collision.gameObject.CompareTag("LineCollider")*/)
        {
            charC.jumping = false;
        }
    }

    private void LineCollisionEnter(Collision2D collision)
    {
        bool condition1 = collision.gameObject.tag == "LineCollider";
        bool condition2 = collision.contacts[0].normal.y < yGroundCheck;

        if (condition1 && condition2)
        {
            coll.layer = LayerMask.NameToLayer("PlayerOff");
            rb.velocity = prevVelocity;
        }
        else if (condition1 && !condition2 && enterAgain)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCorde/LineLand");
            enterAgain = false;
        }
        else if (!condition1)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Land");
        }
    }

    private void GroundCheckCollisionEnter(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > yGroundCheck)
        {
            charC.groundCheck = true;
            if (groundCheckEnum != null)
            {
                StopCoroutine(groundCheckEnum);
                groundCheckEnum = null;
            }
        }
    }
    
    private void CollisionWithPlayer(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && collision.contacts[0].normal.y > .65f)
        {
            rb.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Ball/Bounce");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GroundCheckCollisionStay(collision);
        WallJumpCollisionStay(collision);
    }

    void GroundCheckCollisionStay(Collision2D collision)
    {
        charC.groundCheck = false;
        if (collision.contacts[0].normal.y > yGroundCheck)
        {
            charC.groundCheck = true;
            if (groundCheckEnum != null)
            {
                StopCoroutine(groundCheckEnum);
                groundCheckEnum = null;
            }
        }
    }

    void WallJumpCollisionStay(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > -yWallJump && collision.contacts[0].normal.y < yWallJump && collision.gameObject.CompareTag("Jumpable") /*|| collision.gameObject.CompareTag("LineCollider")*/)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0);
            charC.wallJumpable = collision.contacts[0].normal.x;
            //charC.jumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        groundCheckEnum = waitForGroundCheckOff(charC.ghostInputTimer);
        StartCoroutine(groundCheckEnum);
        StartCoroutine(WaitForSeconds(.2f));

        charC.wallJumpable = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            GetBallOnTriggerEnter(other);
        }
    }

    void GetBallOnTriggerEnter(Collider2D other)
    {
         other.transform.parent.Find("Highlight").gameObject.SetActive(true);
         holdableObjects.Add(other.transform.parent);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "LineCollider")
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerOff");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RemoveBallTriggerExit(other);
        ExitLineTriggerExit(other);
    }

    void RemoveBallTriggerExit(Collider2D other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            other.transform.parent.Find("Highlight").gameObject.SetActive(false);
            holdableObjects.Remove(other.transform.parent);
        }
    }
    
    void ExitLineTriggerExit(Collider2D other)
    {
        bool condition1 = other.tag == "LineCollider";
        if (condition1)
        {
            coll.layer = LayerMask.NameToLayer("Player");
            if (holdingBall)
            {
                coll.layer = LayerMask.NameToLayer("PlayerOff");
            }
        }
    } 
    
    IEnumerator WaitForSeconds(float timer)
    {
        yield return new WaitForSeconds(timer);
        enterAgain = true;
    }

    public IEnumerator waitForGroundCheckOff(float timer)
    {
        yield return new WaitForSeconds(timer);
        charC.groundCheck = false;
    }

    public IEnumerator WaitForPhysics()
    {
        yield return new WaitForFixedUpdate();
        prevVelocity = rb.velocity;
    }

}
