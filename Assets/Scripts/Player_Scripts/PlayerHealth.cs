using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private FlashColor flash;
    [SerializeField] private SignalSender mynotification;
    [SerializeField] public FloatValue health;
    //[SerializeField] private StateMachine myStateMachine;
    [SerializeField] Notification GameOverNotification;
    
    public void FloatDamage(float amountt)
    {
        health.RuntimeValue -= amountt;
        if(health.RuntimeValue <= 0)
        {
            health.RuntimeValue = 0;
        }
        mynotification.Raise();
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);
        if(health.RuntimeValue > 0)
        {
            if (flash)
            {
                flash.StartFlash();
            }

        } else if (health.RuntimeValue <= 0) {
            GameOverNotification.Raise();
        }
    }
}
