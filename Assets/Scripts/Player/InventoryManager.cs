using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public List<ItemData> playerInventory = new List<ItemData>();

    public void AddItem(ItemData item)
    {
        playerInventory.Add(item);
        //update UIManager to instance item sprite to selected vertical layout to show in player display
        UIManager.instance.UpdateItemList(playerInventory);

      
    }

    public void RemoveItem(ItemData item)
    { 
        playerInventory.Remove(item);
        UIManager.instance.UpdateItemList(playerInventory);
    }
}
