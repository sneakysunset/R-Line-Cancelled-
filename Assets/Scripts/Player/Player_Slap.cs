using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class Player_Slap : MonoBehaviour
{
    public float slapStrength;
    private GameObject slapper;
    private WaitForFixedUpdate waiter;
    public float stunLength;
    private void Start()
    {
        slapper = transform.Find("Slapper").gameObject;
        waiter = new WaitForFixedUpdate();
    }

    public void OnSlap(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            slapper.SetActive(true);
            StartCoroutine(endSlapper());
        }
    }

    IEnumerator endSlapper() 
    {
        yield return waiter;
        slapper.SetActive(false);
    }

    public void Slap(Collider2D collision)
    {
        print(collision.name);
        if(collision.transform.root.TryGetComponent<Rigidbody2D>(out Rigidbody2D oRb))
        {
            if (!collision.transform.root.TryGetComponent<Player>(out Player oPlayer))
            {
                oRb.AddForce((collision.transform.position - transform.position).normalized * slapStrength, ForceMode2D.Impulse);
                oPlayer.Stunned(stunLength);
            }
            else if(collision.transform.root.TryGetComponent<Item>(out Item item) && item.itemType == Item.ItemType.holdable && item.isHeld == false)
            {
                oRb.AddForce((collision.transform.position - transform.position).normalized * slapStrength, ForceMode2D.Impulse);
            }
            else if(!collision.transform.root.GetComponent<Item>())
            {
                oRb.AddForce((collision.transform.position - transform.position).normalized * slapStrength, ForceMode2D.Impulse);
            }
        }
    }
}
