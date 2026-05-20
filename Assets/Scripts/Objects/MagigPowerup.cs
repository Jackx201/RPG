 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;

 public class MagigPowerup : PowerUp
 {
     public Inventory playerInventory;
     public float magicValue; 
     public FloatValue playerMagic;
     [SerializeField] private Notification magicNotification;
     

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerMagic.RuntimeValue += magicValue;
            if(playerMagic.RuntimeValue > playerMagic.initialValue)
            {
                playerMagic.RuntimeValue = playerMagic.initialValue;
            }
            magicNotification.Raise();
            Destroy(this.gameObject);
        }
    }
}