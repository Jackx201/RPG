using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Knockback : MonoBehaviour
{

    [SerializeField] string otherTag;
    [SerializeField] float knockTime;
    [SerializeField] float knockStrength;

    public void OnTriggerEnter2D(Collider2D
     other)
    {
        if (other.gameObject.CompareTag(otherTag) && other.isTrigger)
        {
            PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
            if (ph != null && ph.isInvincible) return;

            Rigidbody2D temp = other.GetComponentInParent<Rigidbody2D>();
            if (temp)
            {
                Vector2 direction = other.transform.position - transform.position;
                Vector3 tempdirection = temp.transform.position 
                + (Vector3) direction.normalized * knockStrength;
                
                temp.DOMove(tempdirection, knockTime).SetUpdate(UpdateType.Fixed);
                
                // Set the enemy state to stagger so it doesn't fight the knockback
                Enemmy enemy = other.GetComponentInParent<Enemmy>();
                if (enemy != null)
                {
                    enemy.currentState = EnemyState.stagger;
                    enemy.Knock(temp, knockTime);
                }
            }
        }
    }
}