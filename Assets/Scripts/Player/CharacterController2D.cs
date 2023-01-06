using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CharacterController2D : MonoBehaviour
{
    #region variables non Valued
    PlayerCollisionManager collManager;
    public enum Team { J1, J2, Ball };
    private Rigidbody2D rb;
    [HideInInspector] public Color col;
    /*[HideInInspector]*/ public bool groundCheck = false;
    private float ogGravity;
    bool moveFlag = true;
    bool moving;
    private Vector2 playerVelocity;
    IEnumerator movingEnum;
    public bool jumping;
    private bool wallJumping;
    [HideInInspector]public Vector2 moveValue;
    bool dashing;
    bool dashCDOver = true;
    [HideInInspector] public float wallJumpable;
    private float axx, dxx;
    #endregion
    #region public variables
    [HideInInspector] public bool canDash;
    [HideInInspector] public bool canWallJump;
    [HideInInspector] public bool canJump;
    [HideInInspector] public bool canAirControl;
    [HideInInspector] public bool canFastFall;
    [HideInInspector] public float maxXVelocity = 20;
    [HideInInspector] public float maxYVelocity = 20;
    [HideInInspector] public float ax = 2;
    [HideInInspector] public float dx = 5;
    [HideInInspector] public float wJax = 20;
    [HideInInspector] public float wJdx = 8;
    [HideInInspector] public float dashStrength = 10;
    [HideInInspector] public float dashCoolDown = 1;
    [HideInInspector] public GameObject meshObj;
    [HideInInspector] public Team playerType;
    [HideInInspector] public float jumpStrength;
    [HideInInspector] public float wallJumpStrength;
    [HideInInspector] public Color colorJ1, colorJ2;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float lowJumpMultiplier = 2.5f;
    [HideInInspector] public float fallMultiplier = 2;
    [HideInInspector] public float fastFallStrength = 100;
    [HideInInspector] public float ghostInputTimer;
    [HideInInspector] public float movementScaler;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collManager = GetComponent<PlayerCollisionManager>();
        ogGravity = rb.mass;
        playerTypeChange();
        canMove = true;
        //if (!canJump) canWallJump = false;
    }

    private void playerTypeChange()
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
        if (context.started && (groundCheck || wallJumpable != 0) && canJump)
        {
            jumping = true;
            if (wallJumpable != 0) wallJumping = true;
        }
        else if(context.canceled || context.performed)
        {
            jumping = false;
            wallJumping = false;
        }
    }

    public void OnMove(InputAction.CallbackContext value) => moveValue = value.ReadValue<Vector2>();

    public void OnDash(InputAction.CallbackContext context)
    {
        if(dashCDOver && !dashing && moveValue != Vector2.zero && context.started)
            dashing = true;
    }

    bool flag;
    [HideInInspector] public bool canMove = true;

    private void FixedUpdate()
    {
        if(canMove)
        Move();


        if (groundCheck || !canAirControl)
        {
            axx = ax;
            dxx = dx;
        }
        else if(!groundCheck && wallJumpable != 0)
        {
            axx = wJax;
            dxx = wJdx;
        }
        

        if (jumping && (groundCheck || (wallJumpable != 0 && canWallJump))) Jump();
        else if (!groundCheck)
        {
            if(rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Time.deltaTime * Physics2D.gravity.y * (fallMultiplier - 1);
            }
            else if(rb.velocity.y > 0 && !jumping && !wallJumping)
            {
                rb.velocity += Vector2.up * Time.deltaTime * Physics2D.gravity.y * (lowJumpMultiplier - 1);
            }
/*            else if(rb.velocity.y > 0 && !jumping && !wallJumping)
            {
                rb.velocity += Vector2.up * Time.deltaTime * Physics2D.gravity.y * (20 * lowJumpMultiplier - 1);
            }*/

            if (moveValue.y < -.5f && canFastFall) rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(rb.velocity.x, -fastFallStrength), Time.deltaTime * 2);
        }

        if (moveValue.x < 0) transform.rotation = Quaternion.Euler(0, 180, 0);
        if (moveValue.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);

        if (dashing) Dash();

        //Clamping Speed
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxXVelocity, maxXVelocity), Mathf.Clamp(rb.velocity.y, -1000, maxYVelocity));
    }

   void Jump()
   {
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Jump");
        
        rb.mass = ogGravity;
        rb.AddForce(Vector2.up * jumpStrength + Vector2.right * wallJumpable * wallJumpStrength, ForceMode2D.Impulse);
        
        groundCheck = false;
        StartCoroutine(WaitForPhysics());
        if (collManager.groundCheckEnum != null)
        {
            StopCoroutine(collManager.groundCheckEnum);
            collManager.groundCheckEnum = null;
        }

        //jumping = false;
   }

    private void Dash()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Jump");

        rb.mass = ogGravity;
        
        rb.AddForce(Vector2.right * moveValue.x * dashStrength * ax/ 5, ForceMode2D.Impulse);
        dashing = false;
        dashCDOver = false;
        StartCoroutine(dashCD());
    }

    private void Move()
    {
        Vector2 movementVector = Vector2.zero;
        movementVector.x = moveValue.x * moveSpeed * Time.deltaTime;
        var acc = movementVector.x != 0 ? axx : dxx;


        playerVelocity = Vector2.Lerp(rb.velocity, movementVector, acc * Time.deltaTime);
        playerVelocity.y = rb.velocity.y;
        rb.velocity = playerVelocity;

        if (moveValue != Vector2.zero && groundCheck) moving = true;
        else
        {
            moving = false;
            moveFlag = true;
        }
        if (moveFlag && moving)
        {
            if (movingEnum == null)
            {
                movingEnum = moveSound(.3f);
                StartCoroutine(movingEnum);
            }
            moveFlag = false;
        } 
    }


    public IEnumerator WaitForPhysics()
    {
        yield return new WaitForFixedUpdate();
        groundCheck = false;
    }

    IEnumerator moveSound(float timer)
    {
        //FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Deplacement");
        yield return new WaitForSeconds(timer);
        if (moving)
        {
            movingEnum = moveSound(timer);
            StartCoroutine(movingEnum);
        }
        else
        {
            movingEnum = null;
            moveFlag = true;
        }
    }

    IEnumerator dashCD()
    {
        yield return new WaitForSeconds(dashCoolDown);
        dashCDOver = true;
    }
}

#region editor
#if UNITY_EDITOR

[CustomEditor(typeof(CharacterController2D))/*, ExecuteInEditMode*/]
[System.Serializable]
public class OnGUIEditorHide : Editor
{
    GUIStyle bigTitle, smallTitle, parameter;
    float spaceBetweenTitles = 50;
    float spaceUnderTitle = 10;
    float spaceBetweenParameters = 10;
    public override void OnInspectorGUI()
    {
        Polices();
        GUILayout.Label("Character Controller", bigTitle);
        EditorGUILayout.Space(spaceUnderTitle);

        base.OnInspectorGUI();
        CharacterController2D script = target as CharacterController2D;

        //EditorGUILayout.Space(spaceBetweenTitles);
        GUILayout.Label("Player Abilities", smallTitle);
        EditorGUILayout.Space(spaceUnderTitle);
        script.canJump = EditorGUILayout.Toggle("Can Jump ?", script.canJump);
        script.canWallJump = EditorGUILayout.Toggle("Can WallJump ?", script.canWallJump);
        script.canDash = EditorGUILayout.Toggle("Can Dash ?", script.canDash);
        script.canFastFall = EditorGUILayout.Toggle("Can FastFall ?", script.canFastFall);
        script.canAirControl = EditorGUILayout.Toggle("Can Air Control ?", script.canAirControl);
        EditorGUILayout.Space(spaceBetweenParameters);

        EditorGUILayout.Space(spaceBetweenTitles);
        GUILayout.Label("Player Type", smallTitle);
        EditorGUILayout.Space(spaceUnderTitle);
        GUILayout.Label("Is this player 1 or 2?", parameter);
        script.playerType = (CharacterController2D.Team)EditorGUILayout.EnumPopup("Player Type", script.playerType);
        EditorGUILayout.Space(spaceBetweenParameters);

        bool condition = script.playerType == CharacterController2D.Team.J1;
        GUILayout.Label("The Color of this player material", parameter);
        if (condition) script.colorJ1 = EditorGUILayout.ColorField("Color of Player", script.colorJ1);
        else script.colorJ1 = EditorGUILayout.ColorField("Color of Player", script.colorJ2);
        EditorGUILayout.Space(spaceBetweenParameters);

        EditorGUILayout.Space(spaceBetweenTitles);
        GUILayout.Label("Player Variables", smallTitle);
        EditorGUILayout.Space(spaceUnderTitle);

        GUILayout.Label("The Movement Speed of the player", parameter);
        script.moveSpeed = EditorGUILayout.FloatField("Move Speed", script.moveSpeed);
        EditorGUILayout.Space(spaceBetweenParameters);

        GUILayout.Label("Acceleration and Deceleration of movement. \n 0 disables the movement and 1 makes it 1 second to reach max speed or immobility", parameter);
        script.ax = EditorGUILayout.FloatField("Acceleration", script.ax);
        script.dx = EditorGUILayout.FloatField("Decceleration", script.dx);
        EditorGUILayout.Space(spaceBetweenParameters);

        GUILayout.Label("The Max Velocity the player can reach", parameter);
        script.maxXVelocity = EditorGUILayout.FloatField("Max X Velocity", script.maxXVelocity);
        script.maxYVelocity = EditorGUILayout.FloatField("Max Y Velocity", script.maxYVelocity);
        EditorGUILayout.Space(spaceBetweenParameters);

        if (script.canJump)
        {
            GUILayout.Label("The Strength of this player's Jump", parameter);
            script.jumpStrength = EditorGUILayout.FloatField("Jump Strength", script.jumpStrength);
            EditorGUILayout.Space(spaceBetweenParameters);

            GUILayout.Label("Timer before the player can't jump after leaving the ground", parameter);
            script.ghostInputTimer = EditorGUILayout.FloatField("Ghost Input Timer", script.ghostInputTimer);
            EditorGUILayout.Space(spaceBetweenParameters);

            GUILayout.Label("The higher this value is the faster the player will fall to the ground", parameter);
            script.fallMultiplier = EditorGUILayout.FloatField("fallMultiplier", script.fallMultiplier);
            EditorGUILayout.Space(spaceBetweenParameters);

            GUILayout.Label("The higher this value is the faster the player will fall to the ground", parameter);
            script.lowJumpMultiplier = EditorGUILayout.FloatField("lowJumpMultiplier", script.lowJumpMultiplier);
            EditorGUILayout.Space(spaceBetweenParameters);

            if (script.canWallJump)
        {
            GUILayout.Label("The horizontal Strength of the wall jump", parameter);
            script.wallJumpStrength = EditorGUILayout.FloatField("Wall Jump Strength", script.wallJumpStrength);
            EditorGUILayout.Space(spaceBetweenParameters);
        }

        }

        if (script.canDash)
        {
            GUILayout.Label("The Strength of this player's Dash", parameter);
            script.dashStrength = EditorGUILayout.FloatField("Dash Strength", script.dashStrength);
            EditorGUILayout.Space(spaceBetweenParameters);

            GUILayout.Label("The Length of this player's Dash's Cooldown", parameter);
            script.dashCoolDown = EditorGUILayout.FloatField("Dash Cooldown", script.dashCoolDown);
            EditorGUILayout.Space(spaceBetweenParameters);
        }

        if (script.canFastFall)
        {
            GUILayout.Label("The Speed of the fast fall", parameter);
            script.fastFallStrength = EditorGUILayout.FloatField("Fast Fall Speed", script.fastFallStrength);
            EditorGUILayout.Space(spaceBetweenParameters);
        }

        if (script.canAirControl)
        {
            script.wJax = EditorGUILayout.FloatField("Air Accelation", script.wJax);
            script.wJdx = EditorGUILayout.FloatField("Air Deceleration", script.wJdx);
        }





        //GUILayout.Label("The Speed at which the player scales down when creating line points", parameter);
        //script.movementScaler = EditorGUILayout.FloatField("Movement Scaler", script.movementScaler);
        //EditorGUILayout.Space(spaceBetweenParameters);


        EditorUtility.SetDirty(script); 
    }

    void Polices()
    {
        //These are typo styles used in labels to make the editor more readable
        //They define font size and color but it is also possible to import other fonts onto them

        bigTitle = new GUIStyle(EditorStyles.label);
        bigTitle.normal.textColor = new Color(1, 0.92f, 0.016f, 0.5f);
        bigTitle.fontSize = 20;

        smallTitle = new GUIStyle(EditorStyles.label);
        smallTitle.normal.textColor = Color.white;
        smallTitle.fontSize = 13;

        parameter = new GUIStyle(EditorStyles.label);
        parameter.normal.textColor = Color.grey;
        parameter.fontSize = 12;
    }

}
#endif
#endregion