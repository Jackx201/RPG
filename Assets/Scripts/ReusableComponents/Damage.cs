using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public void ApplyDamage(Health otherHealth, int damageToGive)
    {
        if (otherHealth)
        {
            otherHealth.Damage(damageToGive);
        }
    }

        public void ApplyDamageFloat(PlayerHealth otherHealth, float floatdamage)
    {
        if (otherHealth)
        {
            otherHealth.FloatDamage(floatdamage);
        }
    }
}
