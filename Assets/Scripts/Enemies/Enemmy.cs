using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 - Pasos manuales: 
    - Asignar Target (Usualmente Player)
    - Asignar State Machine del Player
*/
public enum EnemyState{
    idle,
    walk,
    attack,
    stagger,
}
public class Enemmy : MonoBehaviour
{
    //Todos los enemigos tienen estas propiedades
    [Header("StateMachine")]
    public EnemyState currentState;

    [Header("Enemy Stats")]
    public float health;
    public float enemySpeed;
    public string enemyName;
    public int baseAttack;
    public FloatValue maxHealth;
    //public Vector2 homePosition;

    [Header("Efecto de muerte")]    
    public GameObject deathEffect;
    private float deathEffectDelay = 1f;
    public LootTable thisLoot;

    [Header("Death Signals")]
    public SignalSender roomSignal;

    private void Awake(){
        health = maxHealth.initialValue;
    }

    private void OnEnable()
    {
        //transform.position = homePosition;
        health = maxHealth.initialValue;
        currentState = EnemyState.idle;
    }
    
    /* private void TakeDamage(float damage){
        health -= damage;
        if(health <= 0){
            DeathEffect();
            MakeLoot();

            if(roomSignal != null)
            {
            Debug.Log("Enemy defeated, sending room signal.");
            roomSignal.Raise();
            }

            this.gameObject.SetActive(false);
        }
    }*/

    private void MakeLoot()
    {
        if(thisLoot != null)
        {
            PowerUp current = thisLoot.LootPowerUp();
            if(current != null)
            {
                Instantiate(current.gameObject, transform.position, Quaternion.identity);
            }
        }
    }

        private void DeathEffect(){
        if(deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, deathEffectDelay);
        }
    }

    public void Knock(Rigidbody2D myRigidbody, float knockTime)
    {
        StartCoroutine(knockCo(myRigidbody, knockTime));
    }

    private IEnumerator knockCo(Rigidbody2D myRigidbody, float knockTime){
        if(myRigidbody != null){
        yield return new WaitForSeconds(knockTime);
        myRigidbody.linearVelocity = Vector2.zero;
        currentState = EnemyState.idle;
        myRigidbody.linearVelocity = Vector2.zero;
        }
    }
}
