using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : PowerUp
{
    public FloatValue playerHealth;
    public float amountToIncrease;
    public FloatValue healthContainers;
    private float tempHealth;

    public void OnTriggerEnter2D(Collider2D other)
    {
        tempHealth = healthContainers.RuntimeValue * 2f;
        Debug.Log("The max health is currently: " + tempHealth);

        if(other.CompareTag("Player") && !other.isTrigger)
        {
            if((playerHealth.RuntimeValue + amountToIncrease) >= tempHealth)
            {
                playerHealth.RuntimeValue = tempHealth;
            } else {
                playerHealth.RuntimeValue += amountToIncrease;
            }
            if(playerHealth.initialValue > healthContainers.RuntimeValue * 2f)
            {
                playerHealth.initialValue = healthContainers.RuntimeValue *2f;
            }
            powerupSignal.Raise();
            Destroy(this.gameObject);
        }
    }

}
