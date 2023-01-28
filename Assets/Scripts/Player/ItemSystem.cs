using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ItemSystem : MonoBehaviour
{
    [HideInInspector] public Item heldItem;
    bool throwing;
    bool secondaryInputHold;
    GetClosestItem closestItem;
    CharacterController2D charC;
    PlayerCollisionManager pCM;
    public Transform holdPoint;
    public float throwStrength = 15;

    private void Start()
    {
        closestItem = GetComponent<GetClosestItem>();
        charC = GetComponent<CharacterController2D>();
        pCM = GetComponent<PlayerCollisionManager>();
    }

    //normal grab/interaction action
    public void OnItemInput1(InputAction.CallbackContext context)
    {
        if (context.started && heldItem != null)
        {
            heldItem.GrabRelease();
            closestItem.holdableItems.Add(heldItem);
            if (closestItem.closestItem = null) closestItem.closestItem = heldItem;
            heldItem = null;
            return;
        }

        if (context.started && closestItem.closestItem != null && closestItem.closestItem.itemType == Item.ItemType.holdable)
        {
            heldItem = closestItem.closestItem;
            closestItem.holdableItems.Remove(heldItem);
            heldItem.GrabStarted(holdPoint);
            pCM.coll.layer = LayerMask.NameToLayer("PlayerOff");
        }


        if (context.started && closestItem.closestItem != null &&  closestItem.closestItem.itemType == Item.ItemType.interactable && heldItem == null ) heldItem.InteractStarted();
    }

    //normal throw action
    public void OnItemInput2(InputAction.CallbackContext context)
    {
        if (context.started && heldItem != null)
        {
            throwing = true;
            heldItem.ThrowStarted(throwStrength, charC);
        }

        if ((context.canceled || context.performed) && heldItem != null)
        {
            throwing = false;
            if (charC.moveValue == Vector2.zero)
            {
                heldItem.CancelThrow();
                charC.canMove = true;
            }
            else
            {
                heldItem.ThrowRelease(throwStrength, charC);
                closestItem.holdableItems.Add(heldItem);
                heldItem = null;
            }
        }
    }

    //normal secondary action
    public void OnItemInput3(InputAction.CallbackContext context)
    {
        if (context.started && heldItem != null)
        {
            secondaryInputHold = true;
            heldItem.SecondaryInputStarted();
        }

        if ((context.canceled || context.performed) && heldItem != null )
        {
            secondaryInputHold = false;
            heldItem.SecondaryInputReleased();
        }
    }

    private void Update()
    {
        if (heldItem != null && secondaryInputHold) heldItem.SecondaryInputHeld();
        if (heldItem != null && throwing) heldItem.ThrowHeld(throwStrength, charC);
    }
}
