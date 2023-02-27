using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Slap : MonoBehaviour
{
    public float slapStrength;
    public void Slap(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Item>(out Item item) && item.itemType == Item.ItemType.holdable && item.isHeld == false) 
        {
            item.rb.AddForce((transform.position - collision.transform.position).normalized * slapStrength, ForceMode2D.Impulse);
        }
    }
}
