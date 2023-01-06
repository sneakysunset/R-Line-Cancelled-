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

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if(bB != null && context.started /*&& playerCollisionM.holdableObjects.Count == 0*/)
        {
            bRb.isKinematic = false;
            bRb.velocity = Vector2.zero;
            bRb.angularVelocity = 0;
            // bCol.isTrigger = false;

            isHolding = true;   

            if (bB != null)
            {
                bB.tag = "Ball";
                bB.GetComponentInChildren<Collider2D>().tag = "Ball";
            }
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
        else if (playerCollisionM.holdableObjects.Count > 0 && context.started)
        {
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
    }

    bool sim;
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (bB != null && context.started)
        {
            sim = true;
            charC.canMove = false;
        }
        else if(bB != null && (context.performed || context.canceled))
        {
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
            sim = false;
            bRb = null;
            charC.canMove = true;
            isHolding = false;
        }
    }

    private void FixedUpdate()
    {
        if (sim) bBT.Sim(ThrowStrength * charC.moveValue) ;
        //print(sim + name);
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (bB != null)
        {
            bCol.gameObject.layer = 7;
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Grab");

            bRb.isKinematic = false;
           // bCol.isTrigger = false;
            bB.tag = "Ball";
            bB = null;
            bRb = null;
            isHolding = false;
        }
    }
}
