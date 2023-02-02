using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetClosestItem : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        foreach (Item item in player.holdableItems) if (item == null) Debug.LogError("NullItem");

        if (player.holdableItems.Count > 0) player.closestItem = ClosestItem();
        else player.closestItem = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.parent && other.transform.parent.TryGetComponent(out Item item))
        {
            if (item != player.heldItem && !player.holdableItems.Contains(item))
                GetItemOnTriggerEnter(item);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.parent && other.transform.parent.TryGetComponent(out Item item) && player.holdableItems.Contains(item))
        {
            RemoveItemTriggerExit(item);
            if (player.holdableItems.Count == 0)
            {
                item.Highlight.SetActive(false);
                player.closestItem = null;
            }
        }
    }

    void GetItemOnTriggerEnter(Item item) => player.holdableItems.Add(item);

    void RemoveItemTriggerExit(Item item) => player.holdableItems.Remove(item);


    Item ClosestItem()
    {
        Item cItem = null;
        float distance = Mathf.Infinity;

        foreach (Item item in player.holdableItems)
        {
             float itemDistance = Vector2.Distance(item.transform.position, transform.position);
             if (itemDistance < distance)
             {
                 cItem = item;
             }
        }

        if (player.closestItem != null && cItem != player.closestItem)
        {
            player.closestItem.Highlight.SetActive(false);
            cItem.Highlight.SetActive(true);
        }
        else if (player.closestItem == null) cItem.Highlight.SetActive(true);

        return cItem;
    }

}
