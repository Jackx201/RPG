// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public enum DoorType
// {
//     key,
//     enemy,
//     button
// }

// public class Door : Interactable
// {
//     [Header("Door variables")]
//     public DoorType thisDoorType;
//     public bool open = false;
//     public Inventory playerInventory;
//     public SpriteRenderer doorSprite;
//     public BoxCollider2D physicsColider;
    
//     private void Update()
//     {
//         if(Input.GetButtonDown("attack"))
//         {
//             //Is the door locked with key?
//             if (playerInRange && thisDoorType == DoorType.key)
//             {
//                 //Does the player have a key?
//                 if(playerInventory.numberOfKeys > 0)
//                 {
//                     playerInventory.numberOfKeys --;
//                     OpenDoor();
//                 }
//             }
//         }
//     }
//     public void OpenDoor()
//     {
//         doorSprite.enabled = false;
//         open = true;
//         physicsColider.enabled = false;
//     }

//     public void CloseDoor()
//     {
//         doorSprite.enabled = true;
//         open = false;
//         physicsColider.enabled = true;
//     }
// }
