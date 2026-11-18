 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;

 public class DungeonEnemyRoom : DungeonRoom
 {

     public Door[] doors;
     private bool roomActive;

     private void Update()
     {
         if (roomActive && (roomCleared == null || !roomCleared.value))
         {
             CheckEnemies();
         }

        if(roomCleared != null && roomCleared.value){
            OpenDoors();
        }

     }

     public void CheckEnemies()
     {
        Debug.Log("Checking if all enemies are defeated...");
         for( int i = 0; i < enemies.Length; i++)
         {
             if(enemies[i].gameObject.activeInHierarchy)
             {
                 return;
             }
         }
         if (roomCleared != null)
         {
             roomCleared.value = true;
         }
         roomActive = false;
         OpenDoors();
     }

         public override void OnTriggerEnter2D(Collider2D other)
     {
         if(other.CompareTag("Player") && !other.isTrigger)
         {
             int potsquantity = pots.Length;

             if (roomCleared == null || !roomCleared.value)
             {
                 for (int i = 0; i < enemies.Length; i++)
                 {
                     ChangeActive(enemies[i], true);
                 }
                 roomActive = true;
             }

             for (int i=0; i < potsquantity; i++)
             {
                 ChangeActive(pots[i], true);
             }
                 if (roomCleared == null || !roomCleared.value)
                 {
                     CloseDoors();
                 }
                 Debug.Log("Player entered the room, activating camera.");
                virtualCamera.SetActive(true);

                 
         }
     }

     public override void OnTriggerExit2D(Collider2D other)
     {
         if(other.CompareTag("Player") && !other.isTrigger)
         {
             roomActive = false;
             int enemiesquantity = enemies.Length;
             int potsquantity = pots.Length;

             for(int i=0; i< enemiesquantity; i++)
             {
                 ChangeActive(enemies[i], false);
             }

             for (int i=0; i < potsquantity; i++)
             {
                 ChangeActive(pots[i], false);
             }

             virtualCamera.SetActive(false);
             if (roomCleared == null || !roomCleared.value)
             {
                 CloseDoors();
             }
         }
     }

     public void CloseDoors()
     {
         int NumberOfDoors = doors.Length;
         for(int i=0; i < NumberOfDoors; i++)
         {
             doors[i].CloseDoor();
         }
     }

    public void OpenDoors()
     {
         int NumberOfDoors = doors.Length;
         for(int i=0; i < NumberOfDoors; i++)
         {
             doors[i].OpenDoor();
         }
     }
 }
