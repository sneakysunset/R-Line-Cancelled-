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

        //Désactive la variable jumping lors de la collision avec un mur walljumpable.
        if (collision.contacts[0].normal.y > -yWallJump && collision.contacts[0].normal.y < yWallJump && collision.gameObject.CompareTag("Jumpable") /*|| collision.gameObject.CompareTag("LineCollider")*/)
        {
            charC.jumping = false;
        }
    }

    //Logique de collision avec la ligne.
    //Si le y de la normale de la collision est compris entre la valeure "yGroundCheck" entrée en variable publique et cette même valeur négative, /n
    //on traverse la ligne (changement du layer du joueur vers un layer ne détectant pas de collisions avec la ligne).
    //Sinon la ligne arrête le joueur et les sons de collisions sont lancés.
    //Penser à rajouter une variable remplacant "yGroundCheck" dans ce contexte.
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

    //Si le y de la normal de collision avec le sol est inférieur est supérieur à la valeur de "yGroundCheck" rentrée en variable publique rend le booléen groundCheck true.
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
    
    //Si le y de la normale de la collision avec un autre joueur est supérieur à 0.65f fait rebondir le joueur et lance le son de rebond du joueur.
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

    //Si le y de la normal de collision avec le sol est inférieur est supérieur à la valeur de "yGroundCheck" rentrée en variable publique rend le booléen groundCheck true.
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

    //Tant que le joueur reste collé à un mur walljumpable, la variable "wallJumpable" du CharacterController2D est égale à la normale de la collision avec le mur.
    //Le joueur à sa vélocité sur l'axe y égale à 0 pour ne pas glisser le long du mur.
    void WallJumpCollisionStay(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > -yWallJump && collision.contacts[0].normal.y < yWallJump && collision.gameObject.CompareTag("Jumpable") /*|| collision.gameObject.CompareTag("LineCollider")*/)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0);
            charC.wallJumpable = collision.contacts[0].normal.x;
        }
    }

    //Quand le joueur sort d'une collision il lance une coroutine qui après un court timer mets le booléen "groundCheck" en false.
    //Le Vector3 "wallJumpable" est aussi égal à 0 ce qui désactive l'effet de walljump pour le prochain saut.
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

    //Si le joueur rentre dans la zone de trigger autour d'une balle elle rejoint la liste des objets attrapables à proximité.
    void GetBallOnTriggerEnter(Collider2D other)
    {
         other.transform.parent.Find("Highlight").gameObject.SetActive(true);
         holdableObjects.Add(other.transform.parent);
    }

    //Quand la ligne est dans la zone de trigger à l'intérieur du joueur, ce dernier n'a pas de collision avec elle.
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

    //Quand la ligne sort de la zone de trigger à l'intérieur du joueur et que ce dernier ne tient pas de balle, le joueur recommence à rentrer en collision avec la ligne.
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
    
    //Je sais plus honnêtement.
    IEnumerator WaitForSeconds(float timer)
    {
        yield return new WaitForSeconds(timer);
        enterAgain = true;
    }

    //Un petit délai avant de désactiver le ground check du joueur pour qu'il ait le temps de sauter quand il quitte brièvement le sol.
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
