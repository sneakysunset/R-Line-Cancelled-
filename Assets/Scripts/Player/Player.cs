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
    [HideInInspector] public List<Item> holdableItems;
    [HideInInspector] public Item closestItem;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Color col;
    /*[HideInInspector]*/ public bool groundCheck;
    [HideInInspector] public bool jumpingInput;
    /*[HideInInspector] */public bool moving;
    [HideInInspector] public bool throwing;
    [HideInInspector] public bool wallJumpCheck;
    [HideInInspector] public Collider2D coll;
    [HideInInspector] public SpriteRenderer rend;
    [HideInInspector]  public List<Collider2D> mCol;
    //Variables that Show in inspector
    public Team playerType;
    public Color colorJ1, colorJ2;
    public bool canJump;
    public bool canMove;
    public bool noCol;

    #endregion

    public virtual void Start()
    {
        mCol = new List<Collider2D>();
        rend = transform.Find("Renderer").GetComponent<SpriteRenderer>();
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
        if (context.started && (groundCheck || wallJumpCheck) && canJump)
        {
            jumpingInput = true;
        }
        else if (context.canceled || context.performed)
        {
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

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}