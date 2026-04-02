using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPicked : MonoBehaviour
{
    public enum State
    {
        Idle,
        PickedUp
    }

    [SerializeField] State stateCheck = State.Idle ;
    private GameObject Player;
    [SerializeField] private float PickedSpeed = 1f;
    private ItemData itemData;
    [SerializeField] private float DistBeforeDestroy = 1f;
    [SerializeField] private AudioClip PickupAudioClip;

    void Start()
    {
        itemData = GetComponent<ItemData>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        switch (stateCheck)
        {
            case State.PickedUp:
                Vector3 Destination = Player.transform.position;
                Vector3 direction = (Destination - transform.position).normalized;
                transform.position = Vector3.Lerp(transform.position, Destination, PickedSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position,Destination) <= DistBeforeDestroy)
                {
                    //excute command when item reach player
                    if (itemData.itemType == ItemType.Battery)
                    {
                        Player.GetComponent<Flashlight>().AddBattery(itemData.value);
                    }
                    else if (itemData.itemType == ItemType.Goldfish)
                    {
                        Player.transform.GetComponent<InventoryManager>().AddItem(itemData);
                        if (ExitSystem.instance != null)
                        {
                            ExitSystem.instance.ExitProgressUpdate();
                        }
                        

                    }
                    else if (itemData.itemType == ItemType.Key || itemData.itemType == ItemType.Fuse)
                    {
                        Player.transform.GetComponent<InventoryManager>().AddItem(itemData);
                    }
                    AudioManager.instance.PlaySFX(PickupAudioClip);
                    Destroy(gameObject);
                }
                break;

            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && stateCheck != State.PickedUp) {
            stateCheck = State.PickedUp;
        }
    }
}
