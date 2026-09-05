using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private Transform spawnEffectAnchor; // Child object that defines position & layer
    [SerializeField] private LootTable thisLoot;
    [SerializeField] private SignalSender onDeathSignal;

    // Called by the spawner to override the prefab's signal
    public void SetDeathSignal(SignalSender signal)
    {
        onDeathSignal = signal;
    }

    // Called by the spawner to inject the spawn effect
    public void SetSpawnEffect(GameObject effect)
    {
        spawnEffect = effect;
    }

    [SerializeField] private float spawnEffectDuration = 1f; // Fallback if no ParticleSystem found

    private void Start()
    {
        if (spawnEffect != null)
        {
            // Use anchor if assigned, otherwise fall back to root transform
            Transform spawnPoint = spawnEffectAnchor != null ? spawnEffectAnchor : transform;

            GameObject effect = Instantiate(spawnEffect, spawnPoint.position, spawnPoint.rotation, spawnPoint);

            // Unparent so the effect stays in world space when the enemy moves or is disabled
            effect.transform.SetParent(null);

            // Auto-read duration from ParticleSystem, otherwise use fallback
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            float duration = ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : spawnEffectDuration;

            Destroy(effect, duration);
        }
    }

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
