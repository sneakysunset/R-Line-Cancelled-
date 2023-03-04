using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
public class Player : MonoBehaviour
{
    #region variables
    public enum Team { J1, J2, Ball };
    [HideInInspector] public Vector2 moveValue;
    [HideInInspector] public Item heldItem;
    /*[HideInInspector]*/ public List<Item> holdableItems;
    [HideInInspector] public Item closestItem;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Color col;
    [HideInInspector] public bool groundCheck;
    [HideInInspector] public bool jumpingInput;
    [HideInInspector] public bool moving;
    [HideInInspector] public bool throwing;
    [HideInInspector] public bool wallJumpCheck;
    [HideInInspector] public Collider2D coll;
    [HideInInspector] public bool jumpChecker;
    //Variables that Show in inspector
    public Team playerType;
    public Color colorJ1, colorJ2;
    public bool canJump;
    public bool canMove;
    public bool noCol;
    public int numOfJump = 2;
    #endregion

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerTypeChange(); 
        coll = transform.Find("Collider").GetComponent<Collider2D>();
    }

    private void PlayerTypeChange()
    {
        SpriteRenderer pRend = GetComponentInChildren<SpriteRenderer>();
        if (playerType == Team.J1)
        {
            pRend.color = colorJ1;
            col = colorJ1;
        }
        else
        {
            pRend.color = colorJ2;
            col = colorJ2;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && (groundCheck || numOfJump > 0) && canJump)
        {
            jumpingInput = true;
            jumpChecker = true;
        }
        else if (context.canceled || context.performed)
        {
            jumpChecker = false;
            jumpingInput = false;
        }
    }

    public void OnMove(InputAction.CallbackContext value) => moveValue = value.ReadValue<Vector2>();

    public void OnCol(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            noCol = true;
        }
        if(context.canceled || context.performed)
        {
            noCol = false;
        }
    }

    private IEnumerator enume = null;

    public void Stunned(float stunTime)
    {
        canMove = false;
        canJump = false;
        if(enume == null)
        {
            enume = stunEnd(stunTime);
        }
        else
        {
            StopCoroutine(enume);
            enume = stunEnd(stunTime);
        }
        StartCoroutine(enume);

        if(heldItem != null) 
        {
            heldItem.GrabRelease(this);
            holdableItems.Add(heldItem);
            if (closestItem = null) closestItem = heldItem;
            heldItem = null;
        }

    }

    IEnumerator stunEnd(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        canMove = true;
        canJump = true;
        enume = null;
    }
}