 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;

 public class DungeonEnemyRoom : DungeonRoom
 {

     public Door[] doors;

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
         Debug.Log("All enemies defeated, opening doors.");
         OpenDoors();
     }

         public override void OnTriggerEnter2D(Collider2D other)
     {
         if(other.CompareTag("Player") && !other.isTrigger)
         {
             int enemiesquantity = enemies.Length;
             int potsquantity = pots.Length;

             for(int i=0; i< enemiesquantity; i++)
             {
                 ChangeActive(enemies[i], true);
             }

             for (int i=0; i < potsquantity; i++)
             {
                 ChangeActive(pots[i], true);
             }
                 CloseDoors();
                 virtualCamera.SetActive(true);
         }
     }

     public override void OnTriggerExit2D(Collider2D other)
     {
         if(other.CompareTag("Player") && !other.isTrigger)
         {
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
         }
         virtualCamera.SetActive(false);
         CloseDoors();
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
