using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class HoldBall : MonoBehaviour
{
    [HideInInspector] public Transform bB;
    private Rigidbody2D bRb;
    private Collider2D bCol;
    private LineCreator lineC;
    private ThrowPreview bBT;
    private CharacterController2D charC;
    private PlayerCollisionManager playerCollisionM;
    public Transform holdPoint;
    public float ThrowStrength = 5;

    [HideInInspector]
    public bool isHolding;

    private void Awake()
    {
        playerCollisionM = GetComponent<PlayerCollisionManager>();
        charC = GetComponent<CharacterController2D>();
        bB = null;
    }

    //Méthode lancé quand le joueur appuie sur la gachette d'interaction(X sur manette de Xbox).
    //Quand le joueur tient un objet il le lache, si il n'en tient pas et qu'il y en a un à proximité il le porte. Sinon rien ne se passe.
    public void OnInteraction(InputAction.CallbackContext context)
    {
        //Le joueur tient un objet et va donc le lacher. Ce déclenche quand le joueur presse l'input.
        if(bB != null && context.started /*&& playerCollisionM.holdableObjects.Count == 0*/)
        {
            bRb.isKinematic = false;
            bRb.velocity = Vector2.zero;
            bRb.angularVelocity = 0;
            // bCol.isTrigger = false;

            isHolding = true;   

            bB.tag = "Ball";
            bB.GetComponentInChildren<Collider2D>().tag = "Ball";

            playerCollisionM.holdableObjects.Add(bB);
            lineC.lineT.name = "Mesh Ball Free";
            bB = null;
            bRb = null;
            lineC = null;
            bBT = null;

            bCol.gameObject.layer = 7;
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Grab");
            bCol.tag = "Ball";

            playerCollisionM.coll.layer = LayerMask.NameToLayer("PlayerOff");
        }
        //Le joueur ne tient pas d'objet et des objets sont à proximité il va donc attraper le plus proche. Se Déclenche lorsque le joueur appuie sur l'input.
        else if (playerCollisionM.holdableObjects.Count > 0 && context.started)
        {
            //Trouve la balle la plus proche du joueur parmi toutes celles à proximité.
            bB = closestItemFinder(playerCollisionM.holdableObjects);
            bCol = bB.GetComponentInChildren<Collider2D>();
            lineC = bB.GetComponent<LineCreator>();
            bBT = bB.GetComponent<ThrowPreview>();
            lineC.lineT.name = "Mesh Ball Held";
            bCol.gameObject.layer = 14;

            // bCol.isTrigger = true;
            bB.tag = "Held";
            bCol.tag = "Held";
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Catch");
            bB.GetComponentInChildren<Collider2D>().tag = "Held";
            bRb = bB.GetComponent<Rigidbody2D>();
            bRb.isKinematic = true;
            playerCollisionM.holdableObjects.Remove(bB);
        }
    }

    //Retourne le Transform de la balle la plus proche du joueur parmi toutes celles à proximité.
    private Transform closestItemFinder(List<Transform> items)
    {
        Transform closestItem = items[0];
        foreach(Transform item in items)
        {
            if(Vector2.Distance(item.position, transform.position) < Vector2.Distance(closestItem.position, transform.position))
            {
                closestItem = item;
            }
        }
        return closestItem;
    }


    private void Update()
    {
        //Actualise la position de la balle tenue par le joueur si il en tient une et change le layer du joueur.
        if(bB != null && bRb?.isKinematic == true)
        {
            bB.transform.position = holdPoint.position;
            playerCollisionM.holdingBall = true;
            playerCollisionM.coll.layer = LayerMask.NameToLayer("PlayerOff");
        }
        else
        {
            playerCollisionM.holdingBall = false;

        }

        //Lance la méthode de simulation de la trajectoire de la balle.
        if (sim) bBT.Sim(ThrowStrength * charC.moveValue);
    }

    bool sim;
    //Méthode lancé quand le joueur appuie, maintient ou relache l'input de lancé de la balle. 
    public void OnThrow(InputAction.CallbackContext context)
    {
        //Se déclenche quand le joueur presse sur l'input, lance la simulation de la trajectoire de la balle.
        if (bB != null && context.started)
        {
            sim = true;
            var rb = GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(0, rb.velocity.y);
            bBT.ballHolder.gameObject.SetActive(true);
            charC.canMove = false;
        }
        //Se déclenche quand le joueur relache l'input ou quand l'input est annulé. Arrête la simulation de la trajectoire de la balle et lance la balle dans cette même trajectoire.
        else if(bB != null && (context.performed || context.canceled))
        {
            sim = false;
            charC.canMove = true;   
            bBT.ballHolder.gameObject.SetActive(false);
            if (charC.moveValue == Vector2.zero) return;
            bRb.isKinematic = false;
            // bCol.isTrigger = false;
            bCol.gameObject.layer = 7;
            bB.tag = "Ball";
            bCol.tag = "Ball";
            playerCollisionM.holdableObjects.Add(bB);
            bBT._line.positionCount = 1;
            bB = null;
            bBT = null;

            bRb.velocity = GetComponent<CharacterController2D>().moveValue * ThrowStrength;
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Throw");
            // bRb.AddForce(GetComponent<CharacterController2D>().moveValue * ThrowStrength, ForceMode2D.Impulse);
            bRb = null;
            charC.canMove = true;
            isHolding = false;
        }
    }
}
