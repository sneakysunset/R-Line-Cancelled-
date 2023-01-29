using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetClosestItem : MonoBehaviour
{
   [HideInInspector] public List<Item> holdableItems;
   [HideInInspector] public Item closestItem;
    ItemSystem itemS;

    private void Start()
    {
        itemS = GetComponent<ItemSystem>();
    }

    private void Update()
    {
        if (holdableItems.Count > 0) closestItem = ClosestItem();
        else closestItem = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.parent.TryGetComponent<Item>(out Item item) && item != itemS.heldItem)
        {
            GetItemOnTriggerEnter(item);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.parent.TryGetComponent<Item>(out Item item) && holdableItems.Contains(item))
        {
            RemoveItemTriggerExit(item);
            if (holdableItems.Count == 0)
            {
                item.Highlight.SetActive(false);
                closestItem = null;
            }
        }
    }

    void GetItemOnTriggerEnter(Item item) => holdableItems.Add(item);

    void RemoveItemTriggerExit(Item item) => holdableItems.Remove(item);


    Item ClosestItem()
    {
        Item cItem = null;
        float distance = Mathf.Infinity;
        foreach (Item item in holdableItems)
        {
            float itemDistance = Vector2.Distance(item.transform.position, transform.position);
            if (itemDistance < distance)
            {
                cItem = item;
            }
        }

        if (closestItem != null && cItem != closestItem)
        {
            closestItem.Highlight.SetActive(false);
            cItem.Highlight.SetActive(true);
        }
        else if (closestItem == null) cItem.Highlight.SetActive(true);

        return cItem;
    }

}
