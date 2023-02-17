using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class BlockerProperty : MonoBehaviour
{
    public enum BlockerType { Player, Ball, Everything, Nothing, Killer}
    public BlockerType blockerType;
    private SpriteRenderer spriteR;
    private PressurePlate trigger;
    private Collider2D col;
    public Color playerBlockerColor, ballBlockerColor, everythingBlockerColor, nothingBlockerColor, killerBlockerColor;

    private void Start()
    {
        spriteR = GetComponent<SpriteRenderer>();
        trigger = GetComponent<PressurePlate>();
        col = GetComponent<Collider2D>();   
    }

    private void OnValidate()
    {
        spriteR = GetComponent<SpriteRenderer>();
        trigger = GetComponent<PressurePlate>();
        col = GetComponent<Collider2D>();
        ChangeBlockerType(((int)blockerType));
    }

    public void ChangeBlockerType(int _blockerType)
    {
        blockerType = (BlockerType)_blockerType;
        trigger.enabled = false;
        col.enabled = true;
        gameObject.layer = 0;
        switch (blockerType)
        {
            case BlockerType.Player:
                gameObject.layer = 13;
                spriteR.color = playerBlockerColor;
                break; 
            case BlockerType.Ball:
                gameObject.layer = 12;
                spriteR.color = ballBlockerColor;
                break; 
            case BlockerType.Everything:
                spriteR.color = everythingBlockerColor;
                break;  
            case BlockerType.Nothing:
                col.enabled = false;
                spriteR.color = nothingBlockerColor;
                break;
            case BlockerType.Killer:
                gameObject.layer = 12;
                spriteR.color = killerBlockerColor;
                trigger.enabled = true;
                break;
            default:
                spriteR.color = everythingBlockerColor;
                break;
        }
    }
}
