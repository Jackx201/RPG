using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthReaction : MonoBehaviour
{
    public FloatValue playerHealth;
    [SerializeField] FloatValue hearthcontainers;
    public SignalSender healthSignal;


    public void Use(int amountToIncrease)
    {
        float maxHealth = hearthcontainers.RuntimeValue * 2;
        if(playerHealth.RuntimeValue + amountToIncrease >= maxHealth)
        {
            playerHealth.RuntimeValue = maxHealth;
        } else {
            playerHealth.RuntimeValue += amountToIncrease;
        }
        healthSignal.Raise();
    }
}
