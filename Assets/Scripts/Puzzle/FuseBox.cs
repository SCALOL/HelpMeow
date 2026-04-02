using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : MonoBehaviour
{
    public static FuseBox instance;
    [SerializeField] int FusesNeeded = 3;
    [SerializeField] GameObject[] FuseGameObject;
    [SerializeField] private int FuseAmount = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        for (int i = 0; i < FuseGameObject.Length; i++)
        {
            FuseGameObject[i].SetActive(false);
        }

        FusesNeeded = FuseGameObject.Length;


    }
    public void OpenFuseBox(GameObject Inventory)
    {
        InventoryManager inventory = Inventory.GetComponent<InventoryManager>();
        foreach (var i in inventory.playerInventory)
        {
            if (i.itemType == ItemType.Fuse)
            {
                inventory.RemoveItem(i);
                FuseGameObject[FuseAmount].SetActive(true);
                FuseAmount += 1;
                if (FuseAmount >= FusesNeeded)
                {
                    Debug.Log("FuseBox Activated");
                    ExitSystem.instance.ExitProgressUpdate(true);
                }
                Destroy(gameObject);
            }
            break;
        }
    }




    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player in range of FuseBox");
            if (Input.GetButtonDown("Interact"))
            {
                OpenFuseBox(other.transform.parent.gameObject);
            }
        }


    }
}
