using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private LootTable thisLoot;
    [SerializeField] private SignalSender onDeathSignal;


    private void DropLoot()
    {
        if (thisLoot == null) return;
        PowerUp current = thisLoot.LootPowerUp();
        if (current != null)
        {
            Instantiate(current.gameObject, transform.position, Quaternion.identity);
        }
    }


    public override void Damage(int damage)
    {
        base.Damage(damage);
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (onDeathSignal != null)
        {
            onDeathSignal.Raise();
        }

        Instantiate(deathEffect, transform.position, transform.rotation);
        DropLoot();
        this.transform.parent.gameObject.SetActive(false);
    }

}
