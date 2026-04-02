using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] GameObject GoldfishGameObject;

    // Update is called once per frame
    public void OpenChest(GameObject Inventory)
    {
        InventoryManager inventory = Inventory.GetComponent<InventoryManager>();
        foreach (var i in inventory.playerInventory)
        {
            if (i.itemType == ItemType.Key)
            {
                inventory.playerInventory.Remove(i);
                Debug.Log("Chest Opened!");
                // Add logic for giving items to the player here
                Instantiate(GoldfishGameObject, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                Destroy(gameObject);
                break;
            }
            
        }
    }
        

        

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player in range of chest.");
            if (Input.GetButtonDown("Interact"))
            {
                OpenChest(other.gameObject);
            }
        }
            
        
    }
}
