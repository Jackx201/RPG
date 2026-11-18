using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : Interactable
{
    [Header("Door variables")]
    public DoorType thisDoorType;
    public bool open = false;
    public Inventory playerInventory;
    public InventoryItem keyItem;
    public SpriteRenderer[] doorSprites = new SpriteRenderer[4];
    public BoxCollider2D physicsColider;

    private void Update()
    {
        if (Input.GetButtonDown("Check"))
        {
            if (playerInRange && thisDoorType == DoorType.key)
            {
                if (playerInventory.IsItemInInventory(keyItem))
                {
                    playerInventory.UseItem(keyItem);
                    OpenDoor();
                }
            }
        }
    }

    public void OpenDoor()
    {
        for (int i = 0; i < doorSprites.Length; i++)
        {
            if (doorSprites[i] != null)
            {
                doorSprites[i].enabled = false;
            }
        }
        open = true;
        if (physicsColider != null)
        {
            physicsColider.enabled = false;
        }
        this.gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        for (int i = 0; i < doorSprites.Length; i++)
        {
            if (doorSprites[i] != null)
            {
                doorSprites[i].enabled = true;
            }
        }
        open = false;
        if (physicsColider != null)
        {
            physicsColider.enabled = true;
        }
    }
}
