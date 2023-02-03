using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player_ItemSystem : MonoBehaviour
{
    private Player player;
    bool secondaryInputHold;
    Item myItem;
    public Transform holdPoint;
    public float throwStrength = 15;

    private void Start()
    {
        player = GetComponent<Player>();
        myItem = GetComponent<Item>();
    }

    //normal grab/interaction action
    public void OnItemInput1(InputAction.CallbackContext context)
    {
        if (context.started && player.heldItem != null)
        {
            player.heldItem.GrabRelease(player);
            player.holdableItems.Add(player.heldItem);
            if (player.closestItem = null) player.closestItem = player.heldItem;
            player.heldItem = null;
            player.canJump = true;
            player.canMove = true;
            return;
        }

        if (context.started && player.closestItem != null && player.closestItem.itemType == Item.ItemType.holdable)
        {
            player.heldItem = player.closestItem;
            player.holdableItems.Remove(player.heldItem);
            player.heldItem.GrabStarted(holdPoint, player);
            if (player.heldItem is Item_Player) myItem.Highlight.SetActive(false);
        }


        if (context.started && player.closestItem != null && player.closestItem.itemType == Item.ItemType.interactable && player.heldItem == null ) player.closestItem.InteractStarted();
    }

    //normal throw action
    public void OnItemInput2(InputAction.CallbackContext context)
    {
        if (context.started && player.heldItem != null)
        {
            player.throwing = true;
            player.heldItem.ThrowStarted(throwStrength, player);
        }

        if ((context.canceled || context.performed) && player.heldItem != null)
        {
            player.throwing = false;
            if (player.moveValue == Vector2.zero)
            {
                player.heldItem.CancelThrow();
                player.canMove = true;
                player.canJump = true;
            }
            else
            {
                player.holdableItems.Add(player.heldItem);
                player.heldItem.ThrowRelease(throwStrength, player);
                player.heldItem = null;
            }
        }
    }

    //normal secondary action
    public void OnItemInput3(InputAction.CallbackContext context)
    {
        if (context.started && player.heldItem != null)
        {
            secondaryInputHold = true;
            player.heldItem.SecondaryInputStarted();
        }

        if ((context.canceled || context.performed) && player.heldItem != null )
        {
            secondaryInputHold = false;
            player.heldItem.SecondaryInputReleased();
        }
    }

    private void Update()
    {
        if (player.heldItem != null && secondaryInputHold) player.heldItem.SecondaryInputHeld();
        if (player.heldItem != null && player)
        {
            player.heldItem.ThrowHeld(throwStrength, player);
        }
    }
}
