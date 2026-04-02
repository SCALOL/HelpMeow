using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitSystem : MonoBehaviour
{
    [SerializeField] GameObject DimensionDoor;
    public bool CanExit = false;
    [SerializeField] int GoldfishCount = 3; 
    public static ExitSystem instance;
    [SerializeField] GameObject DoorPivot;
    
    [SerializeField] private bool IsFuse;
    [SerializeField] private AudioClip Winaudio;

    void DoorActivate(bool IsOpen)
    {
        if (IsOpen)
        {
            DoorPivot.transform.Rotate(0f, 125f, 0f);
        }
        else
        {
            DoorPivot.transform.Rotate(0f, 0f, 0f);
            
        }
        DimensionDoor.SetActive(IsOpen);
        CanExit = IsOpen;

    }
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        DoorActivate(false);

        
    }

    public void ExitProgressUpdate(bool Fusing = false)
    {
        switch (LevelSceneManager.currentLevel)
        {
            case Level.Level1:
                if (CountGoldfish() >= GoldfishCount)
                {
                    Debug.Log("Enough Goldfish collected");
                    DoorActivate(true);
                }
                break;
            case Level.Level2:
                if (Fusing)
                {
                    IsFuse = true;
                }
                if (CountGoldfish() >= GoldfishCount && IsFuse)
                { 
                    DoorActivate(true);
                }
                break;
        }
        
    }

    private int CountGoldfish()
    {
        //check amount of goldfish in inventory
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        InventoryManager inventory = player.GetComponent<InventoryManager>();
        int GoldfishAmount = 0;
        foreach (var i in inventory.playerInventory)
        {
            if (i.itemType == ItemType.Goldfish)
            {
                GoldfishAmount += 1;
            }
        }
        Debug.Log(GoldfishAmount);
        return GoldfishAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && CanExit)
        {
            //Show WinPage before goback to menu
            if ((int)LevelSceneManager.currentLevel < SceneManager.sceneCountInBuildSettings - 1)
            {
                UIManager.instance.NavigateP("Loading");
                AudioManager.instance.PlaySFX(Winaudio);
                LevelSceneManager.Instance.GotoScene((int)LevelSceneManager.currentLevel + 1);
            }
            else 
            {
                UIManager.instance.NavigateP("Win");
                AudioManager.instance.PlaySFX(Winaudio);
                StartCoroutine(GoBackToMenuAfterDelay(4f));
            }
            
        }
    }

    private IEnumerator GoBackToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LevelSceneManager.Instance.GotoScene(0);
    }
}
