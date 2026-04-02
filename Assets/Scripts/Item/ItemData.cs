using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Goldfish,
    Key,
    Chest,
    Fuse,
    Battery
}

public class ItemData : MonoBehaviour
{
    
    public ItemType itemType;
    public int value;
    public bool Stackable = true;
    public Sprite icon;
    
}
