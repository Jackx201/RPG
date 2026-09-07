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
        Debug.Log($"Knockback OnTriggerEnter2D: {other.gameObject.name}, tag: {other.gameObject.tag}, looking for: {otherTag}");
        if (other.gameObject.CompareTag(otherTag))
        {
            PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
            Debug.Log($"Tag matched. ph null? {ph == null}. Invincible? {(ph != null ? ph.isInvincible.ToString() : "N/A")}");
            if (ph != null && ph.isInvincible) return;

            Rigidbody2D temp = other.GetComponentInParent<Rigidbody2D>();
            Debug.Log($"Rigidbody found? {temp != null}");
            if (temp)
            {
                Vector2 direction = other.transform.position - transform.position;
                Vector3 tempdirection = temp.transform.position 
                + (Vector3) direction.normalized * knockStrength;
                
                temp.DOMove(tempdirection, knockTime).SetUpdate(UpdateType.Fixed);
                
                // Set the enemy state to stagger so it doesn't fight the knockback
                Enemmy enemy = other.GetComponentInParent<Enemmy>();
                Debug.Log($"Enemy found? {enemy != null}");
                if (enemy != null)
                {
                    enemy.currentState = EnemyState.stagger;
                    enemy.Knock(temp, knockTime);
                }
                else
                {
                    PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
                    Debug.Log($"Player found? {player != null}");
                    if (player != null)
                    {
                        Debug.Log("Player Knocked Backkk");
                        player.Knock(knockTime);
                    }
                }
            }
        }
    }
}